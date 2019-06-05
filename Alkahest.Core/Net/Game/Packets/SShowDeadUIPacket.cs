using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_SHOW_DEAD_UI")]
    public sealed class SShowDeadUIPacket : SerializablePacket
    {
        public int Unknown1 { get; set; }

        public int Unknown2 { get; set; }

        public byte Unknown3 { get; set; }

        public int Unknown4 { get; set; }

        public int RevivalItemAmount { get; set; }
    }
}
