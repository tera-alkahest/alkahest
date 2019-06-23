using Alkahest.Core.Data;
using Alkahest.Core.Logging;
using Mono.Options;
using System;
using System.IO;

namespace Alkahest.Commands
{
    sealed class VerifyCommand : AlkahestCommand
    {
        static readonly Log _log = new Log(typeof(VerifyCommand));

        public VerifyCommand()
            : base("Extractor", "verify", "Verify integrity of a decrypted data center file")
        {
            Options = new OptionSet
            {
                $"Usage: {Program.Name} {Name} DDCFILE",
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

            var dc = new DataCenter(File.OpenRead(input), Configuration.DataCenterMode,
                Configuration.DataCenterInterning);

            _log.Info(string.Empty);
            _log.Info("Version: {0}", dc.Header.Version);
            _log.Info("Client Version: {0}", dc.Header.ClientVersion);
            _log.Info(string.Empty);

            static void ForceLoad(DataCenterElement element)
            {
                var dummy = element.Attributes;

                foreach (var child in element.Children())
                    ForceLoad(child);
            }

            ForceLoad(dc.Root);

            _log.Basic("Data center is OK", input);

            return 0;
        }
    }
}
