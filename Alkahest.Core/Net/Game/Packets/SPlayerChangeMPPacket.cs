using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_PLAYER_CHANGE_MP")]
    public sealed class SPlayerChangeMPPacket : SerializablePacket
    {
        public uint CurrentMP { get; set; }

        public uint MaxMP { get; set; }

        public int MPDifference { get; set; }

        public int Unknown1 { get; set; }

        public GameId Target { get; set; }

        public GameId Source { get; set; }
    }
}
