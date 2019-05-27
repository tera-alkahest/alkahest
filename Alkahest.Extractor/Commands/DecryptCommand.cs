using Alkahest.Core.Cryptography;
using Alkahest.Core.IO;
using Alkahest.Core.Logging;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;

namespace Alkahest.Extractor.Commands
{
    sealed class DecryptCommand : ICommand
    {
        static readonly Log _log = new Log(typeof(DecryptCommand));

        public string Name => "decrypt";

        public string Syntax =>
            $"<data center file> <key file> <iv file>";

        public string Description =>
            "Decrypt and uncompress a data center file.";

        public int RequiredArguments => 3;

        public void Run(string output, string[] args)
        {
            var input = args[0];

            if (output == null)
                output = Path.ChangeExtension(input, "dec");

            _log.Basic("Decrypting {0}...", input);

            using var aes = new RijndaelManaged
            {
                Mode = CipherMode.CFB,
                Key = ReadKey(args[1]),
                IV = ReadKey(args[2]),
                Padding = PaddingMode.Zeros,
            };

            using var transform = new PaddingCryptoTransform(aes.CreateDecryptor());
            var stream = new CryptoStream(File.OpenRead(input), transform, CryptoStreamMode.Read);
            using var reader = new BinaryReader(stream, TeraBinaryReader.Encoding, true);

            reader.ReadUInt32();
            reader.ReadUInt16();

            using var stream2 = new DeflateStream(stream, CompressionMode.Decompress);
            using var stream3 = File.Open(output, FileMode.Create, FileAccess.Write);

            stream2.CopyTo(stream3);

            _log.Basic("Decrypted data center to {0}", output);
        }

        static byte[] ReadKey(string path)
        {
            return File.ReadAllLines(path).Single().Split(' ')
                .Select(x => Convert.ToByte(x, 16)).ToArray();
        }
    }
}
