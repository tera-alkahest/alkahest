namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SPartyMemberAbnormalityAddPacket : Packet
    {
        const string Name = "S_PARTY_MEMBER_ABNORMAL_ADD";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SPartyMemberAbnormalityAddPacket();
        }

        [PacketField]
        public uint ServerId { get; set; }

        [PacketField]
        public uint PlayerId { get; set; }

        [PacketField]
        public uint AbnormalityId { get; set; }

        [PacketField]
        public uint Duration { get; set; }

        [PacketField]
        public uint Unknown1 { get; set; }

        [PacketField]
        public uint Stacks { get; set; }
    }
}
