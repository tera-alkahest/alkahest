using System.IO;

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
            if (version != DataCenter.Version)
                throw new InvalidDataException($"Unknown format version {version}.");

            if (unknown1 != 0)
                throw new InvalidDataException($"Unexpected Unknown1 value {unknown1} in header.");

            if (unknown2 != 0)
                throw new InvalidDataException($"Unexpected Unknown2 value {unknown2} in header.");

            if (unknown3 != -16400)
                throw new InvalidDataException($"Unexpected Unknown3 value {unknown3} in header.");

            if (unknown4 != 0)
                throw new InvalidDataException($"Unexpected Unknown4 value {unknown4} in header.");

            if (unknown5 != 0)
                throw new InvalidDataException($"Unexpected Unknown5 value {unknown5} in header.");

            if (unknown6 != 0)
                throw new InvalidDataException($"Unexpected Unknown6 value {unknown6} in header.");

            if (unknown7 != 0)
                throw new InvalidDataException($"Unexpected Unknown7 value {unknown7} in header.");

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
