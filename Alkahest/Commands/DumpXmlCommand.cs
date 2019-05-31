using Alkahest.Core.Data;
using Alkahest.Core.Logging;
using Mono.Options;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Alkahest.Commands
{
    sealed class DumpXmlCommand : AlkahestCommand
    {
        static readonly Log _log = new Log(typeof(DumpXmlCommand));

        string _output = "Xml";

        bool _parallel;

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

            _log.Basic("Dumping {0} as XML...", input);

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

                var i = 0;

                foreach (var elem in grp)
                {
                    using (elem)
                    {
                        using var writer = XmlWriter.Create(Path.Combine(dir, $"{grp.Key}-{i}.xml"),
                            settings);

                        WriteElement(writer, elem);
                        Interlocked.Increment(ref files);
                    }

                    i++;
                };
            });

            _log.Basic("Dumped XML files to directory {0} ({1} directories, {2} files)", _output,
                directories, files);

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
