namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class CCreatePrivateChannelPacket : Packet
    {
        const string Name = "C_CREATE_PRIVATE_CHANNEL";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CCreatePrivateChannelPacket();
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
