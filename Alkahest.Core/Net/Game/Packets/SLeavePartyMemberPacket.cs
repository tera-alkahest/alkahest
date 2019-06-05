using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_LEAVE_PARTY_MEMBER")]
    public sealed class SLeavePartyMemberPacket : SerializablePacket
    {
        public string UserName { get; set; }

        public uint ServerId { get; set; }

        public uint PlayerId { get; set; }
    }
}
