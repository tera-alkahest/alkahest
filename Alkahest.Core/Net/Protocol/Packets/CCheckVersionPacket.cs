namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class CCheckVersionPacket : Packet
    {
        const string Name = "C_CHECK_VERSION";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CCheckVersionPacket();
        }

        [PacketField]
        internal ushort VersionsCount { get; set; }

        [PacketField]
        internal ushort VersionsOffset { get; set; }

        public sealed class Version
        {
            [PacketField]
            public uint Index { get; set; }

            [PacketField]
            public uint Value { get; set; }
        }

        [PacketField]
        public Version[] Versions { get; set; }
    }
}
