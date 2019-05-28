namespace Alkahest.Core.Data
{
    public sealed class DataCenterHeader
    {
        // Some of these are actually more complex structures, but are zeroed.

        public uint Unknown1 { get; }

        public uint Unknown2 { get; }

        public uint Unknown3 { get; }

        public uint Version { get; }

        public uint Unknown4 { get; }

        public uint Unknown5 { get; }

        public uint Unknown6 { get; }

        public uint Unknown7 { get; }

        internal DataCenterHeader(uint unknown1, uint unknown2, uint unknown3, uint version,
            uint unknown4, uint unknown5, uint unknown6, uint unknown7)
        {
            Unknown1 = unknown1;
            Unknown2 = unknown2;
            Unknown3 = unknown3;
            Version = version;
            Unknown4 = unknown4;
            Unknown5 = unknown5;
            Unknown6 = unknown6;
            Unknown7 = unknown7;
        }
    }
}
