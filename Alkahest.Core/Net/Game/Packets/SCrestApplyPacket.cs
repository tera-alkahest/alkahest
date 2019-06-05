using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_CREST_APPLY")]
    public sealed class SCrestApplyPacket : SerializablePacket
    {
        public uint CrestId { get; set; }

        public bool IsActive { get; set; }
    }
}
