using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_HOLD_ABNORMALITY_ADD")]
    public sealed class SHeldAbnormalityAddPacket : SerializablePacket
    {
        public uint Index { get; set; }

        public uint AbnormalityId { get; set; }

        public ulong Duration { get; set; }

        public uint Stacks { get; set; }
    }
}
