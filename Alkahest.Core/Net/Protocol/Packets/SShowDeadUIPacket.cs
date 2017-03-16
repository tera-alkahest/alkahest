namespace Alkahest.Core.Net.Protocol.Packets
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
        public uint Unknown1 { get; set; }

        [PacketField]
        public uint Unknown2 { get; set; }

        [PacketField]
        public uint Unknown3 { get; set; }

        [PacketField]
        public ushort Unknown4 { get; set; }

        [PacketField]
        public ushort Unknown5 { get; set; }

        [PacketField]
        public byte Unknown6 { get; set; }
    }
}
