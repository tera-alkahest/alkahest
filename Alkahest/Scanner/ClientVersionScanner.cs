using System.Linq;

namespace Alkahest.Scanner
{
    sealed class ClientVersionScanner : IScanner
    {
        static readonly byte?[] _pattern1 = new byte?[]
        {
            0x8b, 0x55, null,                   // mov edx, [ebp-0x14]
            0x8b, 0x35, null, null, null, null, // mov esi, <addr>
            0x0f, 0xb7, 0x02,                   // movzx eax, word ptr [edx]
        };

        static readonly byte?[] _pattern2 = new byte?[]
        {
            0x66, 0x83, 0x02, 0x0C,             // add word ptr [edx], 0xC
            0x8B, 0x3D, null, null, null, null, // mov edi, <addr>
        };

        public void Run(MemoryReader reader, IpcChannel channel)
        {
            var o1 = reader.FindOffset(_pattern1).Cast<int?>().FirstOrDefault();
            var o2 = reader.FindOffset(_pattern2).Cast<int?>().FirstOrDefault();

            if (o1 == null || o2 == null)
            {
                channel.LogError("Could not find version reporting function");
                return;
            }

            var ver1 = ReadVersion(reader, (int)o1 + 5);
            var ver2 = ReadVersion(reader, (int)o2 + 6);

            if (ver1 == null || ver2 == null)
            {
                channel.LogError("Could not read version values");
                return;
            }

            channel.LogBasic("Found client versions: {0}, {1}", ver1, ver2);

            channel.WriteVersions((uint)ver1, (uint)ver2);
        }

        static uint? ReadVersion(MemoryReader reader, int offset)
        {
            var off = reader.ReadOffset(offset);

            return reader.IsInRange(off) ? (uint?)reader.Read<uint>(off) : null;
        }
    }
}
