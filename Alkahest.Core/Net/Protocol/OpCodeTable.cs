using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Alkahest.Core.Net.Protocol
{
    public sealed class OpCodeTable
    {
        public Region Region { get; }

        public IReadOnlyDictionary<ushort, string> OpCodeToName { get; }

        public IReadOnlyDictionary<string, ushort> NameToOpCode { get; }

        public OpCodeTable(bool opCodes, Region region)
        {
            Region = region;

            var asm = Assembly.GetExecutingAssembly();
            var name = string.Format(@"Net\Protocol\OpCodes\{0}_{1}.txt",
                opCodes ? "opc" : "smt", region.ToRegionString());
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
