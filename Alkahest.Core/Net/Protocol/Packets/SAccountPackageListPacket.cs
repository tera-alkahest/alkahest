namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SAccountPackageListPacket : Packet
    {
        const string Name = "S_ACCOUNT_PACKAGE_LIST";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SAccountPackageListPacket();
        }

        [PacketField]
        internal ushort PackagesCount { get; set; }

        [PacketField]
        internal ushort PackagesOffset { get; set; }

        public sealed class Package
        {
            [PacketField]
            public uint Unknown2 { get; set; }

            [PacketField]
            public uint Unknown3 { get; set; }

            [PacketField]
            public uint Unknown4 { get; set; }
        }

        [PacketField]
        public Package[] Packages { get; set; }
    }
}
