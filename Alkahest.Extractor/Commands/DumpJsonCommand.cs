using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Alkahest.Core.Data;
using Alkahest.Core.Logging;

namespace Alkahest.Extractor.Commands
{
    sealed class DumpJsonCommand : ICommand
    {
        static readonly Log _log = new Log(typeof(DumpJsonCommand));

        public string Name => "dump-json";

        public string Syntax =>
            $"<data center file>";

        public string Description =>
            "Dump data center contents to a specified directory as JSON.";

        public int RequiredArguments => 1;

        public void Run(string output, string[] args)
        {
            if (output == null)
                output = "Json";

            var input = args[0];

            _log.Basic("Dumping {0} as JSON...", input);

            Directory.CreateDirectory(output);

            using var dc = new DataCenter(input);

            foreach (var grp in dc.Root.GroupBy(x => x.Name))
            {
                var dir = Path.Combine(output, grp.Key);

                Directory.CreateDirectory(dir);

                var i = 0;

                foreach (var elem in grp)
                {
                    using (elem)
                    {
                        using var writer = new JsonTextWriter(
                            new StreamWriter(Path.Combine(dir, $"{grp.Key}-{i}.json")))
                        {
                            Formatting = Formatting.Indented,
                        };

                        WriteElement(writer, elem);
                    }

                    i++;
                }
            }

            _log.Basic("Dumped JSON files to directory {0}", output);
        }

        static void WriteElement(JsonWriter writer, DataCenterElement element)
        {
            writer.WriteStartObject();

            foreach (var attr in element.Attributes)
            {
                writer.WritePropertyName(attr.Name);

                switch (attr.TypeCode)
                {
                    case DataCenterTypeCode.Int32:
                        writer.WriteValue(attr.AsInt32);
                        break;
                    case DataCenterTypeCode.Single:
                        writer.WriteValue(attr.AsSingle);
                        break;
                    case DataCenterTypeCode.String:
                        writer.WriteValue(attr.AsString);
                        break;
                    case DataCenterTypeCode.Boolean:
                        writer.WriteValue(attr.AsBoolean);
                        break;
                }
            }

            foreach (var grp in element.GroupBy(x => x.Name))
            {
                writer.WritePropertyName(grp.Key);
                writer.WriteStartArray();

                foreach (var elem in grp)
                    WriteElement(writer, elem);

                writer.WriteEndArray();
            }

            writer.WriteEndObject();
        }
    }
}
