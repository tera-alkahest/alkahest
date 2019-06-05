using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_ABNORMALITY_REFRESH")]
    public sealed class SAbnormalityRefreshPacket : SerializablePacket
    {
        public GameId Target { get; set; }

        public uint AbnormalityId { get; set; }

        public uint Duration { get; set; }

        public int Unknown1 { get; set; }

        public uint Stacks { get; set; }
    }
}
