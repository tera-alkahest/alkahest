namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SF2PPremiumUserPermissionPacket : Packet
    {
        const string Name = "S_F2P_PremiumUser_Permission";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SF2PPremiumUserPermissionPacket();
        }

        [PacketField]
        internal ushort PermissionsCount { get; set; }

        [PacketField]
        internal ushort PermissionsOffset { get; set; }

        [PacketField]
        public uint Unknown1 { get; set; }

        [PacketField]
        public ushort Unknown2 { get; set; }

        [PacketField]
        public ushort Unknown3 { get; set; }

        [PacketField]
        public ushort Unknown4 { get; set; }

        [PacketField]
        public ushort Unknown5 { get; set; }

        public sealed class Permission
        {
            [PacketField]
            public ushort Unknown6 { get; set; }

            [PacketField]
            public ushort Unknown7 { get; set; }
        }

        [PacketField]
        public Permission[] Permissions { get; set; }
    }
}
