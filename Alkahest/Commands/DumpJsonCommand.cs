using Alkahest.Core.Data;
using Alkahest.Core.Logging;
using Mono.Options;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Alkahest.Commands
{
    sealed class DumpJsonCommand : AlkahestCommand
    {
        static readonly Log _log = new Log(typeof(DumpJsonCommand));

        string _output = "Json";

        bool _parallel;

        public DumpJsonCommand()
            : base("JSON Dumper", "dump-json", "Dump a decrypted data center file as JSON")
        {
            Options = new OptionSet
            {
                $"Usage: {Program.Name} {Name} DDCFILE [OPTIONS]",
                string.Empty,
                "Available options:",
                string.Empty,
                {
                    "o|output=",
                    $"Specify output directory (defaults to `{_output}`)",
                    o => _output = o
                },
                {
                    "p|parallel",
                    $"Parallelize the operation based on number of cores (defaults to `{_parallel}`)",
                    p => _parallel = p != null
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

            _log.Basic("Dumping {0} as JSON...", input);

            Directory.CreateDirectory(_output);

            var directories = 0;
            var files = 0;

            using var dc = new DataCenter(input);
            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = _parallel ? Environment.ProcessorCount : 1,
            };

            Parallel.ForEach(dc.Root.GroupBy(x => x.Name), options, grp =>
            {
                var dir = Path.Combine(_output, grp.Key);

                Directory.CreateDirectory(dir);
                Interlocked.Increment(ref directories);

                var i = 0;

                foreach (var elem in grp)
                {
                    using (elem)
                    {
                        using var writer = new JsonTextWriter(new StreamWriter(
                            Path.Combine(dir, $"{grp.Key}-{i}.json")))
                        {
                            Formatting = Formatting.Indented,
                        };

                        WriteElement(writer, elem);
                        Interlocked.Increment(ref files);
                    }

                    i++;
                }
            });

            _log.Basic("Dumped JSON files to directory {0} ({1} directories, {2} files)", _output,
                directories, files);

            return 0;
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
