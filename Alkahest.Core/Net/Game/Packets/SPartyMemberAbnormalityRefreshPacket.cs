using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SPartyMemberAbnormalityRefreshPacket : Packet
    {
        const string Name = "S_PARTY_MEMBER_ABNORMAL_REFRESH";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SPartyMemberAbnormalityRefreshPacket();
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
