using Alkahest.Core.Data;
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

        internal MessageTable(bool game, uint version)
        {
            if (!DataCenter.Versions.Values.Contains(version))
                throw new ArgumentOutOfRangeException(nameof(version));

            Version = version;

            var asm = Assembly.GetExecutingAssembly();
            var codeToName = new Dictionary<ushort, string>();
            var nameToCode = new Dictionary<string, ushort>();

            using var reader = new StreamReader(asm.GetManifestResourceStream(
                game ? $"protocol.{version}.map" : $"sysmsg.82.map"));

            string line;

            while ((line = reader.ReadLine()) != null)
            {
                var parts = line.Split(' ');
                var name = parts[0];

                // These are just marker values.
                if (!game && (name == "SMT_MAX" || name == "SMT_UNDEFINED"))
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
