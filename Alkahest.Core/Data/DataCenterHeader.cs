namespace Alkahest.Core.Data
{
    public sealed class DataCenterHeader
    {
        // Some of these are actually more complex structures, but are zeroed.

        public uint Version { get; }

        public int Unknown1 { get; }

        public short Unknown2 { get; }

        public short Unknown3 { get; }

        public uint ClientVersion { get; }

        public int Unknown4 { get; }

        public int Unknown5 { get; }

        public int Unknown6 { get; }

        public int Unknown7 { get; }

        internal DataCenterHeader(uint version, int unknown1, short unknown2, short unknown3,
            uint clientVersion, int unknown4, int unknown5, int unknown6, int unknown7)
        {
            Version = version;
            Unknown1 = unknown1;
            Unknown2 = unknown2;
            Unknown3 = unknown3;
            ClientVersion = clientVersion;
            Unknown4 = unknown4;
            Unknown5 = unknown5;
            Unknown6 = unknown6;
            Unknown7 = unknown7;
        }
    }
}
