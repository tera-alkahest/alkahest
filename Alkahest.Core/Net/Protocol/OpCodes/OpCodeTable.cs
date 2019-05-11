using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Alkahest.Core.Net.Protocol.OpCodes
{
    public abstract class OpCodeTable
    {
        public uint Version { get; }

        public IReadOnlyDictionary<ushort, string> OpCodeToName { get; }

        public IReadOnlyDictionary<string, ushort> NameToOpCode { get; }

        public static IReadOnlyDictionary<Region, uint> Versions { get; } =
            new Dictionary<Region, uint>
            {
                { Region.DE, 347373 },
                { Region.FR, 347373 },
                { Region.JP, 347374 },
                { Region.KR, 0 }, // FIXME
                { Region.NA, 347372 },
                { Region.RU, 347375 },
                { Region.TH, 347377 },
                { Region.TW, 347376 },
                { Region.UK, 347373 },
            };

        private protected OpCodeTable(bool opCodes, uint version)
        {
            if (!Versions.Values.Contains(version))
                throw new ArgumentOutOfRangeException(nameof(version));

            Version = version;

            var asm = Assembly.GetExecutingAssembly();
            var resName = $"{(opCodes ? "protocol" : "sysmsg")}.{Version}.map";
            var codeToName = new Dictionary<ushort, string>();
            var nameToCode = new Dictionary<string, ushort>();

            using var reader = new StreamReader(asm.GetManifestResourceStream(resName));

            string line;

            while ((line = reader.ReadLine()) != null)
            {
                var parts = line.Split(' ');
                var name = parts[0];

                // This is just a marker value.
                if (!opCodes && name == "SMT_MAX")
                    continue;

                var code = ushort.Parse(parts[2]);

                codeToName.Add(code, name);
                nameToCode.Add(name, code);
            }

            NameToOpCode = nameToCode;
            OpCodeToName = codeToName;
        }
    }
}
