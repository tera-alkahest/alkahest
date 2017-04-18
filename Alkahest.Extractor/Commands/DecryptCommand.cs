using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using Alkahest.Core.Cryptography;
using Alkahest.Core.IO;

namespace Alkahest.Extractor.Commands
{
    sealed class DecryptCommand : ICommand
    {
        public string Name => "decrypt";

        public string Syntax =>
            $"{Name} <data center file> <key file> <iv file>";

        public string Description =>
            "Decrypt and uncompress a data center file.";

        public int RequiredArguments => 3;

        public void Run(string output, string[] args)
        {
            var input = args[0];

            if (output == null)
                output = Path.ChangeExtension(input, "dec");

            var aes = new RijndaelManaged
            {
                Mode = CipherMode.CFB,
                Key = ReadKey(args[1]),
                IV = ReadKey(args[2]),
                Padding = PaddingMode.Zeros
            };

            var decryptor = new PaddingCryptoTransform(aes.CreateDecryptor());

            using (var stream = new CryptoStream(
                File.OpenRead(input), decryptor, CryptoStreamMode.Read))
            {
                using (var reader = new BinaryReader(stream,
                    TeraBinaryReader.Encoding, true))
                {
                    reader.ReadUInt32();
                    reader.ReadUInt16();
                }

                using (var stream2 = new DeflateStream(stream,
                    CompressionMode.Decompress))
                    using (var stream3 = File.Open(output,
                        FileMode.Create, FileAccess.Write))
                        stream2.CopyTo(stream3);
            }
        }

        static byte[] ReadKey(string path)
        {
            return File.ReadAllLines(path).Single().Split(' ')
                .Select(x => byte.Parse(x, NumberStyles.HexNumber)).ToArray();
        }
    }
}
