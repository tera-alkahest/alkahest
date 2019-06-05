using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Alkahest.Scanner
{
    sealed class GameMessageScanner : IScanner
    {
        delegate IntPtr GetMessageNameFunc(int code);

        static readonly byte?[][] _patterns = new[]
        {
            new byte?[]
            {
                0x55,                         // push ebp
                0x8B, 0xEC,                   // mov ebp, esp
                0x8B, 0x45, 0x08,             // mov eax, [ebp+0x8]
                0x0F, 0xB7, 0xC0,             // movzx eax, ax
                0x3D, 0x88, 0x13, 0x00, 0x00, // cmp eax, 0x1388
            },
            new byte?[]
            {
                0x55,                         // push ebp
                0x8B, 0xEC,                   // mov ebp, esp
                0x0F, 0xB7, 0x45, 0x08,       // movzx eax, [ebp+0x8]
                0x3D, 0x88, 0x13, 0x00, 0x00, // cmp eax, 0x1388
            },
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
            var arr = Enumerable.Range(0, ushort.MaxValue).Select(x => Tuple.Create((ushort)x,
                Marshal.PtrToStringAnsi(func(x)))).Where(x => x.Item2 != string.Empty).ToArray();

            channel.LogBasic("Found {0} game messages", arr.Length);

            channel.WriteGameMessages(arr);
        }
    }
}
