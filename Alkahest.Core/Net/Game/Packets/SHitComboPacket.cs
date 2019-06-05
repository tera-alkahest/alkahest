using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_HIT_COMBO")]
    public sealed class SHitComboPacket : SerializablePacket
    {
        public uint Hits { get; set; }
    }
}
