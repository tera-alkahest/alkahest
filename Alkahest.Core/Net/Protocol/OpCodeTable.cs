using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Alkahest.Core.Net.Protocol
{
    public sealed class OpCodeTable
    {
        public Region Region { get; }

        public IReadOnlyDictionary<ushort, string> OpCodeToName => _names;

        public IReadOnlyDictionary<string, ushort> NameToOpCode => _codes;

        readonly Dictionary<ushort, string> _names =
            new Dictionary<ushort, string>();

        readonly Dictionary<string, ushort> _codes =
            new Dictionary<string, ushort>();

        public OpCodeTable(bool opCodes, Region region)
        {
            Region = region;

            var asm = Assembly.GetExecutingAssembly();
            var name = string.Format(@"Net\Protocol\OpCodes\{0}_{1}.txt",
                opCodes ? "opc" : "smt", region.ToRegionString());

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

                        _names.Add(code, parts[0]);
                        _codes.Add(parts[0], code);
                    }
                }
            }
        }
    }
}
