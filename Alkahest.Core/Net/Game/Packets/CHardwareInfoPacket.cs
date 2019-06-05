using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
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
        public string OperatingSystem { get; set; }

        [PacketField]
        public string Processor { get; set; }

        [PacketField]
        public string GraphicsProcessor { get; set; }

        [PacketField]
        public uint SystemMemory { get; set; }

        [PacketField]
        public uint VideoMemory { get; set; }

        [PacketField]
        public uint ResolutionWidth { get; set; }

        [PacketField]
        public uint ResolutionHeight { get; set; }

        [PacketField]
        public bool IsFullScreen { get; set; }

        [PacketField]
        public uint DisplayResolutionWidth { get; set; }

        [PacketField]
        public uint DisplayResolutionHeight { get; set; }

        [PacketField]
        public uint DisplayCount { get; set; }

        [PacketField]
        public uint SystemResolutionWidth { get; set; }

        [PacketField]
        public uint SystemResolutionHeight { get; set; }

        [PacketField]
        public uint PhysicalCores { get; set; }

        [PacketField]
        public uint LogicalCores { get; set; }
    }
}
