using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Alkahest.Core.Data;

namespace Alkahest.Extractor.Commands
{
    sealed class DumpJsonCommand : Command
    {
        public override string Name { get; } = "dump-json";

        public override string Syntax => $"{Name} <file>";

        public override string Description =>
            "Dump data center contents to a specified directory as JSON.";

        public override int RequiredArguments { get; } = 1;

        public override void Run(string output, string[] args)
        {
            if (output == null)
                output = "Json";

            Directory.CreateDirectory(output);

            using (var dc = new DataCenter(args[0]))
            {
                foreach (var grp in dc.Root.GroupBy(x => x.Name))
                {
                    var dir = Path.Combine(output, grp.Key);

                    Directory.CreateDirectory(dir);

                    var i = 0;

                    foreach (var elem in grp)
                    {
                        using (elem)
                        {
                            using (var writer = new JsonTextWriter(
                                new StreamWriter(Path.Combine(dir,
                                    $"{grp.Key}-{i}.json"))))
                            {
                                writer.Formatting = Formatting.Indented;

                                WriteElement(writer, elem);
                            }
                        }

                        i++;
                    }
                }
            }
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
