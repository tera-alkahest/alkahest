using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_PARTY_MEMBER_CHANGE_MP")]
    public sealed class SPartyMemberChangeMPPacket : SerializablePacket
    {
        public uint ServerId { get; set; }

        public uint PlayerId { get; set; }

        public uint CurrentMP { get; set; }

        public uint MaxMP { get; set; }
    }
}
