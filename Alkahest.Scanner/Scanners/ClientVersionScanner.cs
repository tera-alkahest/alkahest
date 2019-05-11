using System.Linq;

namespace Alkahest.Scanner.Scanners
{
    sealed class ClientVersionScanner : IScanner
    {
        static readonly byte?[] _pattern1 = new byte?[]
        {
            0x8B, 0x55, 0xEC, // mov edx, [ebp - 0x14]
            0x8B, 0x35, null, null, null, null, // mov esi, <addr>
        };

        static readonly byte?[] _pattern2 = new byte?[]
        {
            0x66, 0x83, 0x02, 0x0C, // add word ptr [edx], 0xC
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

            var ver1 = ReadVersion(reader, _pattern1, (int)o1);
            var ver2 = ReadVersion(reader, _pattern2, (int)o2);

            if (ver1 == null || ver2 == null)
            {
                channel.LogError("Could not read version values");
                return;
            }

            channel.LogBasic("Found client versions: {0}, {1}", ver1, ver2);

            channel.Version1 = ver1;
            channel.Version2 = ver2;
        }

        static uint? ReadVersion(MemoryReader reader, byte?[] pattern, int offset)
        {
            var off = reader.ReadOffset(offset + pattern.TakeWhile(x => x != null).Count());

            return reader.IsValid(off) ? (uint?)reader.ReadUInt32(off) : null;
        }
    }
}
