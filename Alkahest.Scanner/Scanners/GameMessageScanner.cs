using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Alkahest.Scanner.Scanners
{
    sealed class GameMessageScanner : IScanner
    {
        delegate IntPtr GetMessageNameFunc(int opCode);

        static readonly byte?[][] _patterns = new[]
        {
            new byte?[]
            {
                0x55, // push ebp
                0x8B, 0xEC, // mov ebp, esp
                0x8B, 0x45, 0x08, // mov eax, [ebp + 0x8]
                0x0F, 0xB7, 0xC0, // movzx eax, ax
                0x3D, 0x88, 0x13, 0x00, 0x00 // cmp eax, 0x1388
            },
            new byte?[]
            {
                0x55, // push ebp
                0x8B, 0xEC, // mov ebp, esp
                0x0F, 0xB7, 0x45, 0x08, // movzx eax, [ebp + 0x8]
                0x3D, 0x88, 0x13, 0x00, 0x00 // cmp eax, 0x1388
            }
        };

        public unsafe void Run(MemoryReader reader, IpcChannel channel)
        {
            var o = reader.FindOffset(_patterns).Cast<int?>().FirstOrDefault();

            if (o == null)
            {
                channel.LogError("Could not find game message name function");
                return;
            }

            var func = reader.GetDelegate<GetMessageNameFunc>((int)o);
            var dict = new Dictionary<ushort, string>();

            for (ushort i = 0; i < ushort.MaxValue; i++)
            {
                string s;

                if ((s = Marshal.PtrToStringAnsi(func(i))) != string.Empty)
                    dict.Add(i, s);
            }

            channel.LogBasic("Found {0} opcodes", dict.Count);

            channel.GameMessages = dict;
        }
    }
}
