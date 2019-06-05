using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("C_HARDWARE_INFO")]
    public sealed class CHardwareInfoPacket : SerializablePacket
    {
        internal static SerializablePacket Create()
        {
            return new CHardwareInfoPacket();
        }

        public string OperatingSystem { get; set; }

        public string Processor { get; set; }

        public string GraphicsProcessor { get; set; }

        public uint SystemMemory { get; set; }

        public uint VideoMemory { get; set; }

        public uint ResolutionWidth { get; set; }

        public uint ResolutionHeight { get; set; }

        public bool IsFullScreen { get; set; }

        public uint DisplayResolutionWidth { get; set; }

        public uint DisplayResolutionHeight { get; set; }

        public uint DisplayCount { get; set; }

        public uint SystemResolutionWidth { get; set; }

        public uint SystemResolutionHeight { get; set; }

        public uint PhysicalCores { get; set; }

        public uint LogicalCores { get; set; }
    }
}
