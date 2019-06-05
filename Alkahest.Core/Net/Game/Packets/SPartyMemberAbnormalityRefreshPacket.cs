using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_PARTY_MEMBER_ABNORMAL_REFRESH")]
    public sealed class SPartyMemberAbnormalityRefreshPacket : SerializablePacket
    {
        public uint ServerId { get; set; }

        public uint PlayerId { get; set; }

        public uint AbnormalityId { get; set; }

        public uint Duration { get; set; }

        public int Unknown1 { get; set; }

        public uint Stacks { get; set; }
    }
}
