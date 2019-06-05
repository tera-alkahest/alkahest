using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("C_BAN_PARTY_MEMBER")]
    public sealed class CBanPartyMemberPacket : SerializablePacket
    {
        public uint ServerId { get; set; }

        public uint PlayerId { get; set; }
    }
}
