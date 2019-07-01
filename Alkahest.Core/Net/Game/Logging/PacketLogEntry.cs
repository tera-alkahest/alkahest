using System;
using System.Collections.Generic;
using System.Text;

namespace Alkahest.Core.Net.Game.Logging
{
    public sealed class PacketLogEntry
    {
        public static IReadOnlyList<byte> Magic { get; } = Encoding.ASCII.GetBytes("TPPL");

        public static uint Version => 1;

        public DateTime Timestamp { get; }

        public int ServerId { get; }

        public Direction Direction { get; }

        public ushort MessageCode { get; }

        public ReadOnlyMemory<byte> Payload { get; }

        public PacketLogEntry(DateTime timestamp, int serverId, Direction direction,
            ushort messageCode, ReadOnlyMemory<byte> payload)
        {
            Timestamp = timestamp;
            ServerId = serverId;
            Direction = direction.CheckValidity(nameof(direction));
            MessageCode = messageCode;
            Payload = payload.ToArray();

            if (payload.Length > PacketHeader.MaxPayloadSize)
                throw new ArgumentException("Payload is too large.", nameof(payload));
        }
    }
}
