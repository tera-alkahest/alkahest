using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_ABNORMALITY_BEGIN")]
    public sealed class SAbnormalityBeginPacket : SerializablePacket
    {
        public GameId Target { get; set; }

        public GameId Source { get; set; }

        public uint AbnormalityId { get; set; }

        public uint Duration { get; set; }

        public int Unknown1 { get; set; }

        public uint Stacks { get; set; }

        public int Unknown2 { get; set; }

        public int Unknown3 { get; set; }
    }
}
