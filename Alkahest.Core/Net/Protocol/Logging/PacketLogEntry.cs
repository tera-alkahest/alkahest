using System;
using System.Collections.Generic;
using System.Linq;

namespace Alkahest.Core.Net.Protocol.Logging
{
    public sealed class PacketLogEntry
    {
        public DateTime Timestamp { get; }

        public string ServerName { get; }

        public Direction Direction { get; }

        public ushort OpCode { get; }

        public IReadOnlyCollection<byte> Payload { get; }

        public PacketLogEntry(DateTime timestamp, string serverName,
            Direction direction, ushort opCode, byte[] payload)
        {
            Timestamp = timestamp;
            ServerName = serverName;
            Direction = direction;
            OpCode = opCode;
            Payload = payload.ToArray();
        }
    }
}
