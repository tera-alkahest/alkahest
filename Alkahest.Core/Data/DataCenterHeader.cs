namespace Alkahest.Core.Data
{
    public sealed class DataCenterHeader
    {
        // Some of these are actually more complex structures, but are zeroed.

        public int Unknown1 { get; }

        public int Unknown2 { get; }

        public int Unknown3 { get; }

        public uint Version { get; }

        public int Unknown4 { get; }

        public int Unknown5 { get; }

        public int Unknown6 { get; }

        public int Unknown7 { get; }

        internal DataCenterHeader(int unknown1, int unknown2, int unknown3, uint version,
            int unknown4, int unknown5, int unknown6, int unknown7)
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
