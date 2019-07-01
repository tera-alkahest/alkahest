namespace Alkahest.Core.Net.Game
{
    public readonly struct PacketHeader
    {
        public static int HeaderSize => sizeof(ushort) * 2;

        public static int MaxPayloadSize => ushort.MaxValue - HeaderSize;

        public static int MaxPacketSize => HeaderSize + MaxPayloadSize;

        public readonly ushort Length { get; }

        public readonly ushort Code { get; }

        public ushort FullLength => (ushort)(Length + HeaderSize);

        public PacketHeader(ushort length, ushort code)
        {
            Length = length;
            Code = code;
        }

        public override string ToString()
        {
            return $"[Length: {Length}, Code: {Code}]";
        }
    }
}
