using Alkahest.Core.Collections;
using Alkahest.Core.Data;
using Alkahest.Core.Logging;
using Alkahest.Parser;
using Mono.Options;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Alkahest.Commands
{
    sealed class DumpCommand : AlkahestCommand
    {
        static readonly Log _log = new Log(typeof(DumpCommand));

        DumpFormat _format = DumpFormat.Xml;

        string _output = "Dump";

        bool _parallel;

        public DumpCommand()
            : base("Extractor", "dump", "Dump a decrypted data center file as XML/JSON")
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
                    "f|format=",
                    $"Specify dump format (defaults to `{_format}`)",
                    (DumpFormat m) => _format = m
                },
                {
                    "p|parallel",
                    $"Enable/disable parallelizing the operation based on number of cores (defaults to `{_parallel}`)",
                    p => _parallel = p != null
                },
                string.Empty,
                "Dump formats:",
                string.Empty,
                $"  {DumpFormat.Xml}: Dump data as XML (default)",
                $"  {DumpFormat.Json}: Dump data as JSON",
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

            _log.Basic("Dumping {0} as {1} to directory {2}...", input, _format, _output);

            Directory.CreateDirectory(_output);

            var directories = 0;
            var files = 0;

            using var dc = new DataCenter(input);
            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = _parallel ? Environment.ProcessorCount : 1,
            };
            var settings = new XmlWriterSettings
            {
                Indent = true,
            };

            Parallel.ForEach(dc.Root.GroupBy(x => x.Name), options, grp =>
            {
                var dir = Path.Combine(_output, grp.Key);

                Directory.CreateDirectory(dir);
                Interlocked.Increment(ref directories);

                foreach (var (i, elem) in grp.WithIndex())
                {
                    using (elem)
                    {
                        switch (_format)
                        {
                            case DumpFormat.Xml:
                            {
                                using var writer = XmlWriter.Create(Path.Combine(
                                    dir, $"{grp.Key}-{i}.xml"), settings);

                                WriteElement(writer, elem);
                                break;
                            }
                            case DumpFormat.Json:
                            {
                                using var writer = new JsonTextWriter(new StreamWriter(
                                    Path.Combine(dir, $"{grp.Key}-{i}.json")))
                                {
                                    Formatting = Newtonsoft.Json.Formatting.Indented,
                                };

                                WriteElement(writer, elem);
                                break;
                            }
                        }
                    }

                    Interlocked.Increment(ref files);
                };
            });

            _log.Basic("Dumped {0} directories and {1} files", directories, files);

            return 0;
        }

        static void WriteElement(XmlWriter writer, DataCenterElement element)
        {
            writer.WriteStartElement(element.Name);

            foreach (var (name, attr) in element.Attributes.Tuples())
                writer.WriteAttributeString(name, attr.Value.ToString());

            foreach (var elem in element)
                WriteElement(writer, elem);

            writer.WriteEndElement();
        }

        static void WriteElement(JsonWriter writer, DataCenterElement element)
        {
            writer.WriteStartObject();

            foreach (var (name, attr) in element.Attributes.Tuples())
            {
                writer.WritePropertyName(name);

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
