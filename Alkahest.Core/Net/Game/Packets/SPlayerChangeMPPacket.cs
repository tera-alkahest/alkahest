using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SPlayerChangeMPPacket : Packet
    {
        const string Name = "S_PLAYER_CHANGE_MP";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SPlayerChangeMPPacket();
        }

        [PacketField]
        public uint CurrentMP { get; set; }

        [PacketField]
        public uint MaxMP { get; set; }

        [PacketField]
        public int MPDifference { get; set; }

        [PacketField]
        public int Unknown1 { get; set; }

        [PacketField]
        public GameId Target { get; set; }

        [PacketField]
        public GameId Source { get; set; }
    }
}
