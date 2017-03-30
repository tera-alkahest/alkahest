using System;
using System.Collections.Generic;
using System.Linq;

namespace Alkahest.Core.Net.Protocol.Logging
{
    public sealed class PacketLogEntry
    {
        public static IReadOnlyList<byte> Magic => _magic;

        public static int Version { get; } = 0;

        static readonly byte[] _magic =
            new[] { 'A', 'T', 'P', 'L' }.Select(x => (byte)x).ToArray();

        public DateTime Timestamp { get; }

        public int ServerId { get; }

        public Direction Direction { get; }

        public ushort OpCode { get; }

        public IReadOnlyList<byte> Payload { get; }

        public PacketLogEntry(DateTime timestamp, int serverId,
            Direction direction, ushort opCode, byte[] payload)
        {
            Timestamp = timestamp;
            ServerId = serverId;
            Direction = direction;
            OpCode = opCode;
            Payload = payload.ToArray();
        }
    }
}
