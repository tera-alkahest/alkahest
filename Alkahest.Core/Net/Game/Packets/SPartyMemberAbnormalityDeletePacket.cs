namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SPartyMemberAbnormalityDeletePacket : Packet
    {
        const string Name = "S_PARTY_MEMBER_ABNORMAL_DEL";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SPartyMemberAbnormalityDeletePacket();
        }

        [PacketField]
        public uint ServerId { get; set; }

        [PacketField]
        public uint PlayerId { get; set; }

        [PacketField]
        public uint AbnormalityId { get; set; }
    }
}
