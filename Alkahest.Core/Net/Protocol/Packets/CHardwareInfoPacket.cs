namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class CHardwareInfoPacket : Packet
    {
        const string Name = "C_HARDWARE_INFO";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CHardwareInfoPacket();
        }

        [PacketField]
        internal ushort OperatingSystemOffset { get; set; }

        [PacketField]
        internal ushort ProcessorOffset { get; set; }

        [PacketField]
        internal ushort GraphicsProcessorOffset { get; set; }

        [PacketField]
        public uint SystemMemory { get; set; }

        [PacketField]
        public uint Unknown2 { get; set; }

        [PacketField]
        public uint ResolutionWidth { get; set; }

        [PacketField]
        public uint ResolutionHeight { get; set; }

        [PacketField]
        public uint Unknown3 { get; set; }

        [PacketField]
        public uint Unknown4 { get; set; }

        [PacketField]
        public uint Unknown5 { get; set; }

        [PacketField]
        public uint Unknown6 { get; set; }

        [PacketField]
        public uint Unknown7 { get; set; }

        [PacketField]
        public uint Unknown8 { get; set; }

        [PacketField]
        public uint Unknown9 { get; set; }

        [PacketField]
        public byte Unknown10 { get; set; }

        [PacketField]
        public string OperatingSystem { get; set; }

        [PacketField]
        public string Processor { get; set; }

        [PacketField]
        public string GraphicsProcessor { get; set; }
    }
}
