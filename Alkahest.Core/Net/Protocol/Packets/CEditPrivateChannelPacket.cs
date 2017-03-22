namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class CEditPrivateChannelPacket : Packet
    {
        const string Name = "C_EDIT_PRIVATE_CHANNEL";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CEditPrivateChannelPacket();
        }

        [PacketField]
        internal ushort MembersCount { get; set; }

        [PacketField]
        internal ushort MembersOffset { get; set; }

        [PacketField]
        internal ushort ChannelNameOffset { get; set; }

        [PacketField]
        public ushort Password { get; set; }

        [PacketField]
        public string ChannelName { get; set; }

        public sealed class Member
        {
            [PacketField]
            public uint PlayerId { get; set; }
        }

        [PacketField]
        public Member[] Members { get; set; }
    }
}
