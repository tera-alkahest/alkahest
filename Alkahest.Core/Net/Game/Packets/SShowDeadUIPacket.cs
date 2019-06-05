using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SShowDeadUIPacket : Packet
    {
        const string Name = "S_SHOW_DEAD_UI";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SShowDeadUIPacket();
        }

        [PacketField]
        public int Unknown1 { get; set; }

        [PacketField]
        public int Unknown2 { get; set; }

        [PacketField]
        public byte Unknown3 { get; set; }

        [PacketField]
        public int Unknown4 { get; set; }

        [PacketField]
        public int RevivalItemAmount { get; set; }
    }
}
