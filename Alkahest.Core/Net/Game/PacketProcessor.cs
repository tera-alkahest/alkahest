using Alkahest.Core.IO;
using Alkahest.Core.Logging;
using Alkahest.Core.Net.Game.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Alkahest.Core.Net.Game
{
    public sealed class PacketProcessor
    {
        static readonly Log _log = new Log(typeof(PacketProcessor));

        public PacketSerializer Serializer { get; }

        readonly HashSet<RawPacketHandler> _wildcardRawHandlers = new HashSet<RawPacketHandler>();

        readonly Dictionary<ushort, HashSet<RawPacketHandler>> _rawHandlers =
            new Dictionary<ushort, HashSet<RawPacketHandler>>();

        readonly Dictionary<ushort, HashSet<Delegate>> _handlers =
           new Dictionary<ushort, HashSet<Delegate>>();

        static readonly IReadOnlyCollection<Delegate> _emptyHandlers = new List<Delegate>();

        static readonly IReadOnlyCollection<RawPacketHandler> _emptyRawHandlers = new List<RawPacketHandler>();

        readonly object _listLock = new object();

        readonly object _invokeLock = new object();

        public PacketProcessor(PacketSerializer serializer)
        {
            Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));

            foreach (var code in serializer.GameMessages.CodeToName.Keys)
            {
                _rawHandlers.Add(code, new HashSet<RawPacketHandler>());
                _handlers.Add(code, new HashSet<Delegate>());
            }
        }

        ushort GetCode(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (!Serializer.GameMessages.NameToCode.TryGetValue(name, out var op))
                throw new InvalidOperationException($"Game message {name} is not mapped to a code.");

            return op;
        }

        static string GetName(Type type)
        {
            return type.GetCustomAttribute<PacketAttribute>().Name;
        }

        public void AddRawHandler(RawPacketHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            lock (_listLock)
                _wildcardRawHandlers.Add(handler);
        }

        public void RemoveRawHandler(RawPacketHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            lock (_listLock)
                _wildcardRawHandlers.Remove(handler);
        }

        public void AddRawHandler(string name, RawPacketHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var code = GetCode(name);

            lock (_listLock)
                _rawHandlers[code].Add(handler);
        }

        public void RemoveRawHandler(string name, RawPacketHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var code = GetCode(name);

            lock (_listLock)
                _rawHandlers[code].Remove(handler);
        }

        public void AddHandler<T>(PacketHandler<T> handler)
            where T : SerializablePacket
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var code = GetCode(GetName(typeof(T)));

            lock (_listLock)
                _handlers[code].Add(handler);
        }

        public void RemoveHandler<T>(PacketHandler<T> handler)
            where T : SerializablePacket
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var code = GetCode(GetName(typeof(T)));

            lock (_listLock)
                _handlers[code].Remove(handler);
        }

        internal static PacketHeader ReadHeader(GameBinaryReader reader)
        {
            reader.Position = 0;

            var length = (ushort)(reader.ReadUInt16() - PacketHeader.HeaderSize);
            var code = reader.ReadUInt16();

            return new PacketHeader(length, code);
        }

        internal static void WriteHeader(GameBinaryWriter writer, PacketHeader header)
        {
            writer.Position = 0;

            writer.WriteUInt16((ushort)(header.Length + PacketHeader.HeaderSize));
            writer.WriteUInt16(header.Code);
        }

        bool RunHandlers(GameClient client, Direction direction, ref PacketHeader header,
            ref Memory<byte> payload, string name)
        {
            IReadOnlyCollection<RawPacketHandler> rawHandlers;

            // Make a copy so we don't have to lock while iterating.
            lock (_listLock)
            {
                var registered = _rawHandlers[header.Code];

                if (_wildcardRawHandlers.Count != 0 || registered.Count != 0)
                {
                    var rh = new List<RawPacketHandler>();

                    rh.AddRange(_wildcardRawHandlers);
                    rh.AddRange(registered);

                    rawHandlers = rh;
                }
                else
                    rawHandlers = _emptyRawHandlers;
            }

            var send = true;

            if (rawHandlers.Count != 0)
            {
                var packet = new RawPacket(name)
                {
                    Payload = payload.Slice(0, header.Length),
                };

                foreach (var handler in rawHandlers)
                {
                    try
                    {
                        lock (_invokeLock)
                            send &= handler(client, direction, packet);
                    }
                    catch (Exception e) when (!Debugger.IsAttached)
                    {
                        _log.Error("Unhandled exception in raw packet handler:");
                        _log.Error("{0}", e);
                    }
                }

                payload = packet.Payload.ToArray();
                header = new PacketHeader((ushort)packet.Payload.Length, header.Code);
            }

            IReadOnlyCollection<Delegate> handlers = _handlers[header.Code];

            lock (_listLock)
                handlers = handlers.Count != 0 ? handlers.ToArray() : _emptyHandlers;

            if (handlers.Count != 0)
            {
                var packet = Serializer.Create(header.Code);
                var good = true;

                try
                {
                    var seg = payload.GetArray();

                    Serializer.Deserialize(seg.Array, seg.Offset, seg.Count, packet);
                }
                catch (EndOfStreamException)
                {
                    _log.Error("{0}: {1} failed to deserialize; skipping typed packet handlers",
                        direction.ToDirectionString(), name);
                    good = false;
                }

                if (good)
                {
                    foreach (var handler in handlers)
                    {
                        try
                        {
                            lock (_invokeLock)
                                send &= (bool)handler.DynamicInvoke(client, direction, packet);
                        }
                        catch (Exception e) when (!Debugger.IsAttached)
                        {
                            _log.Error("Unhandled exception in packet handler:");
                            _log.Error("{0}", e);
                        }
                    }

                    payload = Serializer.Serialize(packet);
                    header = new PacketHeader((ushort)payload.Length, header.Code);
                }
            }

            return send;
        }

        internal bool Process(GameClient client, Direction direction, ref PacketHeader header,
            ref Memory<byte> payload)
        {
            Serializer.GameMessages.CodeToName.TryGetValue(header.Code, out var name);

            var send = true;
            var original = payload;

            if (name != null)
                send = RunHandlers(client, direction, ref header, ref payload, name);
            else
                name = header.Code.ToString();

            _log.Debug("{0}: {1} ({2} bytes{3})", direction.ToDirectionString(), name, header.Length,
                send ? string.Empty : ", discarded");

            if (send && payload.Length > PacketHeader.MaxPayloadSize)
            {
                _log.Error("{0}: {1} is too large ({2} bytes) to be sent correctly; sending original",
                    direction.ToDirectionString(), name, payload.Length);

                payload = original;
                header = new PacketHeader((ushort)payload.Length, header.Code);
            }

            return send;
        }
    }
}
