using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_PARTY_MEMBER_ABNORMAL_CLEAR")]
    public sealed class SPartyMemberAbnormalityClearPacket : SerializablePacket
    {
        public uint ServerId { get; set; }

        public uint PlayerId { get; set; }
    }
}
