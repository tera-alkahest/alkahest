using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Alkahest.Core.Net.Game
{
    public abstract class MessageTable
    {
        public uint Version { get; }

        public IReadOnlyDictionary<ushort, string> CodeToName { get; }

        public IReadOnlyDictionary<string, ushort> NameToCode { get; }

        public static IReadOnlyDictionary<Region, uint> Versions { get; } =
            new Dictionary<Region, uint>
            {
                { Region.DE, 350022 },
                { Region.FR, 350022 },
                { Region.JP, 347374 },
                { Region.KR, 0 },
                { Region.NA, 347372 },
                { Region.RU, 347375 },
                { Region.TH, 349932 },
                { Region.TW, 347376 },
                { Region.UK, 350022 },
            };

        private protected MessageTable(bool opCodes, uint version)
        {
            if (!Versions.Values.Contains(version))
                throw new ArgumentOutOfRangeException(nameof(version));

            Version = version;

            var asm = Assembly.GetExecutingAssembly();
            var codeToName = new Dictionary<ushort, string>();
            var nameToCode = new Dictionary<string, ushort>();

            using var reader = new StreamReader(asm.GetManifestResourceStream(
                opCodes ? $"protocol.{version}.map" : $"sysmsg.{(version >= 349932 ? 82 : 81)}.map"));

            string line;

            while ((line = reader.ReadLine()) != null)
            {
                var parts = line.Split(' ');
                var name = parts[0];

                // These are just marker values.
                if (!opCodes && (name == "SMT_MAX" || name == "SMT_UNDEFINED"))
                    continue;

                var code = ushort.Parse(parts[parts[1] == "=" ? 2 : 1]);

                codeToName.Add(code, name);
                nameToCode.Add(name, code);
            }

            NameToCode = nameToCode;
            CodeToName = codeToName;
        }
    }
}
