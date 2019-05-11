using System.IO;
using System.Linq;
using System.Xml;
using Alkahest.Core.Data;
using Alkahest.Core.Logging;

namespace Alkahest.Extractor.Commands
{
    sealed class DumpXmlCommand : ICommand
    {
        static readonly Log _log = new Log(typeof(DumpXmlCommand));

        public string Name => "dump-xml";

        public string Syntax =>
            $"<data center file>";

        public string Description =>
            "Dump data center contents to a specified directory as XML.";

        public int RequiredArguments => 1;

        public void Run(string output, string[] args)
        {
            if (output == null)
                output = "Xml";

            var input = args[0];

            _log.Basic("Dumping {0} as XML...", input);

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
                        var settings = new XmlWriterSettings
                        {
                            Indent = true,
                        };

                        using var writer = XmlWriter.Create(
                            Path.Combine(dir, $"{grp.Key}-{i}.xml"), settings);

                        WriteElement(writer, elem);
                    }

                    i++;
                }
            }

            _log.Basic("Dumped XML files to directory {0}", output);
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
