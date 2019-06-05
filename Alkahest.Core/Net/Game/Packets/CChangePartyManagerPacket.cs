using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("C_CHANGE_PARTY_MANAGER")]
    public sealed class CChangePartyManagerPacket : SerializablePacket
    {
        public uint ServerId { get; set; }

        public uint PlayerId { get; set; }
    }
}
