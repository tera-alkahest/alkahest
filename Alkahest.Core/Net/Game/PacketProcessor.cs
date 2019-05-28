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
        const BindingFlags CreateFlags =
            BindingFlags.DeclaredOnly |
            BindingFlags.NonPublic |
            BindingFlags.Static;

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

        ushort GetOpCode(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (!Serializer.GameMessages.NameToCode.TryGetValue(name, out var op))
                throw new ArgumentException("Invalid opcode name.", nameof(name));

            return op;
        }

        static string GetOpCodeName(Type t)
        {
            var name = t.GetMethod("Create", CreateFlags, null, Type.EmptyTypes, null)
                ?.GetCustomAttribute<PacketAttribute>()?.OpCode;

            if (name == null)
                throw new ArgumentException("Invalid packet type.", "handler");

            return name;
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

            var opCode = GetOpCode(name);

            lock (_listLock)
                _rawHandlers[opCode].Add(handler);
        }

        public void RemoveRawHandler(string name, RawPacketHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var opCode = GetOpCode(name);

            lock (_listLock)
                _rawHandlers[opCode].Remove(handler);
        }

        public void AddHandler<T>(PacketHandler<T> handler)
            where T : Packet
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var opCode = GetOpCode(GetOpCodeName(typeof(T)));

            lock (_listLock)
                _handlers[opCode].Add(handler);
        }

        public void RemoveHandler<T>(PacketHandler<T> handler)
            where T : Packet
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var opCode = GetOpCode(GetOpCodeName(typeof(T)));

            lock (_listLock)
                _handlers[opCode].Remove(handler);
        }

        internal static PacketHeader ReadHeader(byte[] buffer)
        {
            using var reader = new GameBinaryReader(buffer);

            var length = (ushort)(reader.ReadUInt16() - PacketHeader.HeaderSize);
            var opCode = reader.ReadUInt16();

            return new PacketHeader(length, opCode);
        }

        internal static void WriteHeader(PacketHeader header, byte[] buffer)
        {
            using var writer = new GameBinaryWriter(buffer);

            writer.WriteUInt16((ushort)(header.Length + PacketHeader.HeaderSize));
            writer.WriteUInt16(header.OpCode);
        }

        bool RunHandlers(GameClient client, Direction direction, ref PacketHeader header,
            ref byte[] payload, string name)
        {
            IReadOnlyCollection<RawPacketHandler> rawHandlers;

            // Make a copy so we don't have to lock while iterating.
            lock (_listLock)
            {
                var registered = _rawHandlers[header.OpCode];

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

                payload = packet.Payload;
                header = new PacketHeader((ushort)packet.Payload.Length, header.OpCode);
            }

            IReadOnlyCollection<Delegate> handlers = _handlers[header.OpCode];

            lock (_listLock)
                handlers = handlers.Count != 0 ? handlers.ToArray() : _emptyHandlers;

            if (handlers.Count != 0)
            {
                var packet = Serializer.Create(header.OpCode);
                var good = true;

                try
                {
                    Serializer.Deserialize(payload.Slice(0, header.Length), packet);
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
                    header = new PacketHeader((ushort)payload.Length, header.OpCode);
                }
            }

            return send;
        }

        internal bool Process(GameClient client, Direction direction, ref PacketHeader header,
            ref byte[] payload)
        {
            Serializer.GameMessages.CodeToName.TryGetValue(header.OpCode, out var name);

            var send = true;
            var original = payload;

            if (name != null)
                send = RunHandlers(client, direction, ref header, ref payload, name);
            else
                name = header.OpCode.ToString();

            _log.Debug("{0}: {1} ({2} bytes{3})", direction.ToDirectionString(), name, header.Length,
                send ? string.Empty : ", discarded");

            if (send && payload.Length > PacketHeader.MaxPayloadSize)
            {
                _log.Error("{0}: {1} is too big ({2} bytes) to be sent correctly; sending original",
                    direction.ToDirectionString(), name, payload.Length);

                payload = original;
                header = new PacketHeader((ushort)payload.Length, header.OpCode);
            }

            return send;
        }
    }
}
