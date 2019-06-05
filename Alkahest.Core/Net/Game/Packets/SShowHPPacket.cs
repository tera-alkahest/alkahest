using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SShowHPPacket : Packet
    {
        const string Name = "S_SHOW_HP";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SShowHPPacket();
        }

        [PacketField]
        public GameId Target { get; set; }

        [PacketField]
        public ulong CurrentHP { get; set; }

        [PacketField]
        public ulong MaxHP { get; set; }

        [PacketField]
        public bool IsEnemy { get; set; }

        [PacketField]
        public int Unknown1 { get; set; }

        [PacketField]
        public int Unknown2 { get; set; }

        [PacketField]
        public int Unknown3 { get; set; }

        [PacketField]
        public int Unknown4 { get; set; }
    }
}
