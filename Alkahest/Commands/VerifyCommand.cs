using Alkahest.Core.Data;
using Alkahest.Core.Logging;
using Mono.Options;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Alkahest.Commands
{
    sealed class VerifyCommand : AlkahestCommand
    {
        static readonly Log _log = new Log(typeof(VerifyCommand));

        bool _sum;

        public VerifyCommand()
            : base("Extractor", "verify", "Verify integrity of a decrypted data center file")
        {
            Options = new OptionSet
            {
                $"Usage: {Program.Name} {Name} DDCFILE [OPTIONS]",
                string.Empty,
                "Available options:",
                string.Empty,
                {
                    "s|sum",
                    $"Enable/disable computing and printing the SHA sums for the file (defaults to `{_sum}`)",
                    s => _sum = s != null
                },
            };
        }

        protected override int Invoke(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Expected exactly 1 argument");
                return 1;
            }

            var input = args[0];

            _log.Basic("Verifying {0}...", input);

            using var stream = File.OpenRead(input);
            var dc = new DataCenter(stream, Configuration.DataCenterMode,
                Configuration.DataCenterStringOptions);

            _log.Info(string.Empty);
            _log.Info("File Version: {0}", dc.Header.Version);
            _log.Info("Client Version: {0}", dc.Header.ClientVersion);
            _log.Info("Patch Version: {0}", dc.Root.Children("BuildVersion").Single()["version"].AsInt32);
            _log.Info(string.Empty);

            using var sha1 = new SHA1Managed();
            using var sha256 = new SHA256Managed();
            using var sha384 = new SHA384Managed();
            using var sha512 = new SHA512Managed();

            if (_sum)
            {
                void PrintHash(string name, HashAlgorithm algorithm)
                {
                    stream.Position = 0;

                    var hash = algorithm.ComputeHash(stream);
                    var sb = new StringBuilder(hash.Length * 2);

                    foreach (var b in hash)
                        sb.Append(b.ToString("x2"));

                    _log.Info("{0}: {1}", name, sb);
                }

                PrintHash("SHA-1", sha1);
                PrintHash("SHA-256", sha256);
                PrintHash("SHA-384", sha384);
                PrintHash("SHA-512", sha512);
                _log.Info(string.Empty);
            }

            var elements = 0;
            var attributes = 0;

            void ForceLoad(DataCenterElement element)
            {
                elements++;
                attributes += element.Attributes.Count;

                foreach (var child in element.Children())
                    ForceLoad(child);
            }

            ForceLoad(dc.Root);

            _log.Basic("Verified {0} elements and {1} attributes", elements, attributes);

            return 0;
        }
    }
}
