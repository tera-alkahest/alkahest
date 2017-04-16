using System;
using System.Collections.Generic;
using System.Linq;

namespace Alkahest.Core.Net.Protocol.Logging
{
    public sealed class PacketLogEntry
    {
        public static IReadOnlyList<byte> Magic { get; } =
            new[] { 'A', 'T', 'P', 'L' }.Select(x => (byte)x).ToArray();

        public static int Version { get; } = 0;

        public DateTime Timestamp { get; }

        public int ServerId { get; }

        public Direction Direction { get; }

        public ushort OpCode { get; }

        public IReadOnlyList<byte> Payload { get; }

        public PacketLogEntry(DateTime timestamp, int serverId,
            Direction direction, ushort opCode, byte[] payload)
        {
            direction.CheckValidity(nameof(direction));

            Timestamp = timestamp;
            ServerId = serverId;
            Direction = direction;
            OpCode = opCode;
            Payload = (payload ??
                throw new ArgumentNullException(nameof(payload))).ToArray();
        }
    }
}
