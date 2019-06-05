using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("C_EXTEND_PARTY")]
    public sealed class CExtendPartyPacket : SerializablePacket
    {
        public bool IsRaid { get; set; }
    }
}
