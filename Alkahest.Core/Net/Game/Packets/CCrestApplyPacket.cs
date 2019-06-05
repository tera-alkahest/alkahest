using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("C_CREST_APPLY")]
    public sealed class CCrestApplyPacket : SerializablePacket
    {
        public uint CrestId { get; set; }

        public bool IsActive { get; set; }
    }
}
