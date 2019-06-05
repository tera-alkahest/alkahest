using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_CREATURE_CHANGE_HP")]
    public sealed class SCreatureChangeHPPacket : SerializablePacket
    {
        public ulong CurrentHP { get; set; }

        public ulong MaxHP { get; set; }

        public long HPDifference { get; set; }

        public int Unknown1 { get; set; }

        public GameId Target { get; set; }

        public GameId Source { get; set; }

        public byte Unknown2 { get; set; }

        public uint AbnormalityId { get; set; }
    }
}
