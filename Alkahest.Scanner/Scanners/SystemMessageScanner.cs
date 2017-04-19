using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Alkahest.Scanner.Scanners
{
    sealed class SystemMessageScanner : IScanner
    {
        delegate IntPtr GetMessageNameFunc(uint index);

        static readonly byte?[] _pattern = new byte?[]
        {
            0x55, // push ebp
            0x8B, 0xEC, // mov ebp, esp
            0x8B, 0x45, 0x08, // mov eax, [ebp + 0x8]
            0x85, 0xC0, // test eax, eax
            0x78, 0x10, // js short 0xA
            0x3D, null, null, null, null // cmp eax, <count>
        };

        public void Run(MemoryReader reader, IpcChannel channel)
        {
            var o = reader.FindOffset(_pattern).Cast<int?>().FirstOrDefault();

            if (o == null)
            {
                channel.LogError("Could not find system message table");
                return;
            }

            var count = reader.ReadUInt32((int)o +
                _pattern.TakeWhile(x => x != null).Count());
            var func = reader.GetDelegate<GetMessageNameFunc>((int)o);
            var dict = new Dictionary<ushort, string>();

            for (ushort i = 0; i < count; i++)
                dict.Add(i, Marshal.PtrToStringUni(func(i)));

            channel.LogBasic("Found {0} system messages", dict.Count);

            channel.SystemMessages = dict;
        }
    }
}
