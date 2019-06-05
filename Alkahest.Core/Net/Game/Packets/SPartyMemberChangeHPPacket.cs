using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_PARTY_MEMBER_CHANGE_HP")]
    public sealed class SPartyMemberChangeHPPacket : SerializablePacket
    {
        public uint ServerId { get; set; }

        public uint PlayerId { get; set; }

        public ulong CurrentHP { get; set; }

        public ulong MaxHP { get; set; }
    }
}
