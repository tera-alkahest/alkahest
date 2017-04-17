using System.IO;
using System.Linq;
using SharpDisasm;
using SharpDisasm.Udis86;
using Alkahest.Core.Data;

namespace Alkahest.Scanner.Scanners
{
    sealed class DataCenterKeyScanner : IScanner
    {
        static readonly byte?[] _pattern = new byte?[]
        {
            0x56, // push esi
            0x57, // push edi
            0x50, // push eax
            0x8D, 0x45, 0xF4, // lea eax, [ebp - 0xC]
            0x64, 0xA3, 0x00, 0x00, 0x00, 0x00, // mov fs:[0x0], eax
            0x8B, 0x73, 0x08, // mov esi, [ebx + 0x8]
            0x8B, 0xCE // mov ecx, esi
        };

        public void Run(MemoryReader reader, IpcChannel channel)
        {
            var o = reader.FindOffset(_pattern).Cast<int?>().FirstOrDefault();

            if (o == null)
            {
                channel.LogError("Could not find data center decryption function");
                return;
            }

            using (var disasm = new Disassembler(reader.ToAbsolute((int)o),
                reader.Length, ArchitectureMode.x86_32,
                (ulong)reader.BaseAddress))
            {
                channel.DataCenterKey = ReadKey(disasm);
                channel.DataCenterIV = ReadKey(disasm);
            }
        }

        static byte[] ReadKey(Disassembler disassembler)
        {
            var stream = new MemoryStream(DataCenter.KeySize);

            using (var writer = new BinaryWriter(stream))
            {
                while (stream.Position < stream.Capacity)
                {
                    var insn = disassembler.NextInstruction();

                    if (insn.Mnemonic == ud_mnemonic_code.UD_Imov &&
                        insn.Operands[0].Base == ud_type.UD_R_EBP)
                        writer.Write(insn.Operands[1].LvalUDWord);
                }

                writer.Flush();

                return stream.ToArray();
            }
        }
    }
}
