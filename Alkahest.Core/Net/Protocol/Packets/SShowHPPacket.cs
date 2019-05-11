using Alkahest.Core.Game;

namespace Alkahest.Core.Net.Protocol.Packets
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
        public float CurrentHP { get; set; }

        [PacketField]
        public float MaxHP { get; set; }

        [PacketField]
        public bool IsEnemy { get; set; }

        [PacketField]
        public uint Unknown1 { get; set; }

        [PacketField]
        public uint Unknown2 { get; set; }

        [PacketField]
        public uint Unknown3 { get; set; }

        [PacketField]
        public uint Unknown4 { get; set; }
    }
}
