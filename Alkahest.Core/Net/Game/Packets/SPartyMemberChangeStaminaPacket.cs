using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_PARTY_MEMBER_CHANGE_STAMINA")]
    public sealed class SPartyMemberChangeStaminaPacket : SerializablePacket
    {
        public uint ServerId { get; set; }

        public uint PlayerId { get; set; }

        public uint CurrentResource { get; set; }

        public uint MaxResource { get; set; }

        public int Unknown1 { get; set; }
    }
}
