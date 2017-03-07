namespace Alkahest.Core.Net.Protocol
{
    public struct PacketHeader
    {
        public const int HeaderSize = sizeof(ushort) * 2;

        public const int MaxPayloadSize = ushort.MaxValue - HeaderSize;

        public const int MaxPacketSize = HeaderSize + MaxPayloadSize;

        public readonly ushort Length;

        public readonly ushort OpCode;

        public ushort FullLength => (ushort)(Length + HeaderSize);

        public PacketHeader(ushort length, ushort opCode)
        {
            Length = length;
            OpCode = opCode;
        }
    }
}
