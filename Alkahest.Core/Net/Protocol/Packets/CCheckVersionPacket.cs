namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class CCheckVersionPacket : Packet
    {
        const string Name = "C_CHECK_VERSION";

        public override string OpCode
        {
            get { return Name; }
        }

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CCheckVersionPacket();
        }

        [PacketField]
        internal ushort VersionValuesCount { get; set; }

        [PacketField]
        internal ushort VersionValuesOffset { get; set; }

        public sealed class VersionValue
        {
            [PacketField]
            public int Index { get; set; }

            [PacketField]
            public int Value { get; set; }
        }

        [PacketField]
        public VersionValue[] VersionValues { get; set; }
    }
}
