using Alkahest.Core.Data;
using Alkahest.Core.Logging;
using Mono.Options;
using System;
using System.IO;
using System.Linq;
using System.Xml;

namespace Alkahest.Commands
{
    sealed class DumpXmlCommand : AlkahestCommand
    {
        static readonly Log _log = new Log(typeof(DumpXmlCommand));

        string _output = "Json";

        public DumpXmlCommand()
            : base("XML Dumper", "dump-xml", "Dump a decrypted data center file as XML")
        {
            Options = new OptionSet
            {
                $"Usage: {Program.Name} {Name} DDCFILE [OPTIONS]",
                string.Empty,
                "Available options:",
                string.Empty,
                {
                    "o|output",
                    $"Specify output directory (defaults to `{_output}`)",
                    o => _output = o
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

            _log.Basic("Dumping {0} as XML...", input);

            Directory.CreateDirectory(_output);

            using var dc = new DataCenter(input);

            foreach (var grp in dc.Root.GroupBy(x => x.Name))
            {
                var dir = Path.Combine(_output, grp.Key);

                Directory.CreateDirectory(dir);

                var i = 0;

                foreach (var elem in grp)
                {
                    using (elem)
                    {
                        var settings = new XmlWriterSettings
                        {
                            Indent = true,
                        };

                        using var writer = XmlWriter.Create(Path.Combine(dir, $"{grp.Key}-{i}.xml"),
                            settings);

                        WriteElement(writer, elem);
                    }

                    i++;
                }
            }

            _log.Basic("Dumped XML files to directory {0}", _output);

            return 0;
        }

        static void WriteElement(XmlWriter writer, DataCenterElement element)
        {
            writer.WriteStartElement(element.Name);

            foreach (var attr in element.Attributes)
                writer.WriteAttributeString(attr.Name, attr.Value.ToString());

            foreach (var elem in element)
                WriteElement(writer, elem);

            writer.WriteEndElement();
        }
    }
}
