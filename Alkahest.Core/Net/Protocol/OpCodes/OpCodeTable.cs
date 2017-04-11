using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Alkahest.Core.Net.Protocol.OpCodes
{
    public abstract class OpCodeTable
    {
        public int Version { get; }

        public IReadOnlyDictionary<ushort, string> OpCodeToName { get; }

        public IReadOnlyDictionary<string, ushort> NameToOpCode { get; }

        public static IReadOnlyDictionary<Region, int> Versions { get; } =
            new Dictionary<Region, int>
            {
                { Region.EU, 311383 },
                { Region.JP, 311383 },
                { Region.KR, 313577 },
                { Region.NA, 311383 },
                { Region.RU, 311383 },
                { Region.TW, 311383 }
            };

        internal OpCodeTable(bool opCodes, int version)
        {
            Version = version;

            var asm = Assembly.GetExecutingAssembly();
            var name = string.Format(@"Net\Protocol\OpCodes\{0}_{1}.txt",
                opCodes ? "opc" : "smt", Version);
            var codeToName = new Dictionary<ushort, string>();
            var nameToCode = new Dictionary<string, ushort>();

            using (var stream = asm.GetManifestResourceStream(name))
            {
                using (var reader = new StreamReader(stream))
                {
                    string line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        var parts = line.Split(' ');

                        // This is just a marker value.
                        if (!opCodes && parts[0] == "SMT_MAX")
                            continue;

                        var code = ushort.Parse(parts[2]);

                        codeToName.Add(code, parts[0]);
                        nameToCode.Add(parts[0], code);
                    }
                }
            }

            NameToOpCode = nameToCode;
            OpCodeToName = codeToName;
        }
    }
}
