using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Alkahest.Core.IO;
using Alkahest.Core.Logging;
using Alkahest.Core.Net.Protocol.Logging;

namespace Alkahest.Core.Net.Protocol
{
    public sealed class PacketProcessor
    {
        const BindingFlags CreateFlags =
            BindingFlags.DeclaredOnly |
            BindingFlags.NonPublic |
            BindingFlags.Static;

        static readonly Log _log = new Log(typeof(PacketProcessor));

        public PacketSerializer Serializer { get; }

        public PacketLogWriter LogWriter { get; }

        readonly HashSet<RawPacketHandler> _wildcardRawHandlers =
            new HashSet<RawPacketHandler>();

        readonly Dictionary<ushort, HashSet<RawPacketHandler>> _rawHandlers =
            new Dictionary<ushort, HashSet<RawPacketHandler>>();

        readonly Dictionary<ushort, HashSet<Delegate>> _handlers =
           new Dictionary<ushort, HashSet<Delegate>>();

        readonly object _lock = new object();

        public PacketProcessor(PacketSerializer serializer,
            PacketLogWriter logWriter)
        {
            Serializer = serializer;
            LogWriter = logWriter;

            foreach (var code in serializer.GameMessages.OpCodeToName.Keys)
            {
                _rawHandlers.Add(code, new HashSet<RawPacketHandler>());
                _handlers.Add(code, new HashSet<Delegate>());
            }
        }

        public void AddRawHandler(RawPacketHandler handler)
        {
            lock (_lock)
                _wildcardRawHandlers.Add(handler);
        }

        public void RemoveRawHandler(RawPacketHandler handler)
        {
            lock (_lock)
                _wildcardRawHandlers.Remove(handler);
        }

        public void AddRawHandler(string name, RawPacketHandler handler)
        {
            lock (_lock)
                _rawHandlers[Serializer.GameMessages.NameToOpCode[name]].Add(handler);
        }

        public void RemoveRawHandler(string name, RawPacketHandler handler)
        {
            lock (_lock)
                _rawHandlers[Serializer.GameMessages.NameToOpCode[name]].Remove(handler);
        }

        static string GetOpCode(Type t)
        {
            return t.GetMethod("Create", CreateFlags, null, Type.EmptyTypes, null)
                .GetCustomAttribute<PacketAttribute>().OpCode;
        }

        public void AddHandler<T>(PacketHandler<T> handler)
            where T : Packet
        {
            lock (_lock)
                _handlers[Serializer.GameMessages.NameToOpCode[GetOpCode(typeof(T))]].Add(handler);
        }

        public void RemoveHandler<T>(PacketHandler<T> handler)
            where T : Packet
        {
            lock (_lock)
                _handlers[Serializer.GameMessages.NameToOpCode[GetOpCode(typeof(T))]].Remove(handler);
        }

        internal static PacketHeader ReadHeader(byte[] buffer)
        {
            using (var reader = new TeraBinaryReader(buffer))
            {
                var length = (ushort)(reader.ReadUInt16() - PacketHeader.HeaderSize);
                var opCode = reader.ReadUInt16();

                return new PacketHeader(length, opCode);
            }
        }

        internal static void WriteHeader(PacketHeader header, byte[] buffer)
        {
            using (var writer = new TeraBinaryWriter(buffer))
            {
                writer.Write((ushort)(header.Length + PacketHeader.HeaderSize));
                writer.Write(header.OpCode);
            }
        }

        internal bool Process(GameClient client, Direction direction,
            ref PacketHeader header, ref byte[] payload)
        {
            var rawHandlers = new List<RawPacketHandler>();

            // Make a copy so we don't have to lock while iterating.
            lock (_lock)
            {
                rawHandlers.AddRange(_wildcardRawHandlers);
                rawHandlers.AddRange(_rawHandlers[header.OpCode]);
            }

            var send = true;
            var name = Serializer.GameMessages.OpCodeToName[header.OpCode];

            if (rawHandlers.Count != 0)
            {
                var packet = new RawPacket(name)
                {
                    Payload = payload.Slice(0, header.Length)
                };

                foreach (var handler in rawHandlers)
                {
                    try
                    {
                        send &= handler(client, direction, packet);
                    }
                    catch (Exception e)
                    {
                        if (Debugger.IsAttached)
                            throw;

                        _log.Error("Unhandled exception in raw packet handler:");
                        _log.Error(e.ToString());
                    }
                }

                payload = packet.Payload;
                header = new PacketHeader((ushort)packet.Payload.Length, header.OpCode);
            }

            IReadOnlyCollection<Delegate> handlers = _handlers[header.OpCode];

            lock (_lock)
                handlers = handlers.Count != 0 ? handlers.ToArray() : null;

            if (handlers != null)
            {
                var packet = Serializer.Create(header.OpCode);

                Serializer.Deserialize(payload, packet);

                foreach (var handler in handlers)
                {
                    try
                    {
                        send &= (bool)handler.DynamicInvoke(client,
                            direction, packet);
                    }
                    catch (Exception e)
                    {
                        if (Debugger.IsAttached)
                            throw;

                        _log.Error("Unhandled exception in packet handler:");
                        _log.Error(e.ToString());
                    }
                }

                payload = Serializer.Serialize(packet);
                header = new PacketHeader((ushort)payload.Length, header.OpCode);
            }

            LogWriter?.Write(new PacketLogEntry(DateTime.Now,
                client.Proxy.Info.Name, direction, header.OpCode,
                payload.Slice(0, header.Length)));

            _log.Debug("{0}: {1} ({2} bytes{3})", direction.ToDirectionString(),
                name, header.Length, send ? string.Empty : ", discarded");

            return send;
        }
    }
}
