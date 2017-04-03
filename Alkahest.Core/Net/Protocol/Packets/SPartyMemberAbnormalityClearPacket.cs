namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SPartyMemberAbnormalityClearPacket : Packet
    {
        const string Name = "S_PARTY_MEMBER_ABNORMAL_CLEAR";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SPartyMemberAbnormalityClearPacket();
        }

        [PacketField]
        public uint ServerId { get; set; }

        [PacketField]
        public uint PlayerId { get; set; }
    }
}
