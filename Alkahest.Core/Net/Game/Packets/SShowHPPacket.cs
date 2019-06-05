using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_SHOW_HP")]
    public sealed class SShowHPPacket : SerializablePacket
    {
        public GameId Target { get; set; }

        public ulong CurrentHP { get; set; }

        public ulong MaxHP { get; set; }

        public bool IsEnemy { get; set; }

        public int Unknown1 { get; set; }

        public int Unknown2 { get; set; }

        public int Unknown3 { get; set; }

        public int Unknown4 { get; set; }
    }
}
