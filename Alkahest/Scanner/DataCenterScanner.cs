using Alkahest.Core.Data;
using SharpDisasm;
using SharpDisasm.Udis86;
using System.IO;
using System.Linq;

namespace Alkahest.Scanner
{
    sealed class DataCenterScanner : IScanner
    {
        static readonly byte?[] _pattern = new byte?[]
        {
            0x56,                               // push esi
            0x57,                               // push edi
            0x50,                               // push eax
            0x8D, 0x45, 0xF4,                   // lea eax, [ebp-0xC]
            0x64, 0xA3, 0x00, 0x00, 0x00, 0x00, // mov large fs:0x0, eax
            0x8B, 0x73, 0x08,                   // mov esi, [ebp+0x8]
        };

        public void Run(MemoryReader reader, IpcChannel channel)
        {
            var o = reader.FindOffset(_pattern).Cast<int?>().FirstOrDefault();

            if (o == null)
            {
                channel.LogError("Could not find data center decryption function");
                return;
            }

            var off = (int)o + _pattern.Length;

            using var disasm = new Disassembler(reader.ToAbsolute(off),
                reader.Length - off, ArchitectureMode.x86_32,
                (ulong)reader.Address, true);

            var key = ReadKey(disasm);
            var iv = ReadKey(disasm);

            if (key == null || iv == null)
            {
                channel.LogError("Could not find data center key/IV");
                return;
            }

            channel.LogBasic("Found data center key: {0}", StringizeKey(key));
            channel.LogBasic("Found data center IV: {0}", StringizeKey(iv));

            channel.DataCenterKey = key;
            channel.DataCenterIV = iv;
        }

        static string StringizeKey(byte[] key)
        {
            return string.Join(" ", key.Select(x => x.ToString("X2")));
        }

        static byte[] ReadKey(Disassembler disassembler)
        {
            var stream = new MemoryStream(DataCenter.KeySize);
            using var writer = new BinaryWriter(stream);

            while (stream.Position < stream.Capacity)
            {
                var insn = disassembler.NextInstruction();

                if (insn == null || insn.Error)
                    return null;

                if (insn.Mnemonic == ud_mnemonic_code.UD_Imov &&
                    insn.Operands[0].Base == ud_type.UD_R_EBP)
                    writer.Write(insn.Operands[1].LvalUDWord);
            }

            writer.Flush();

            return stream.ToArray();
        }
    }
}
