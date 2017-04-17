using System.IO;
using System.Linq;
using System.Xml;
using Alkahest.Core.Data;

namespace Alkahest.Extractor.Commands
{
    sealed class DumpXmlCommand : ICommand
    {
        public string Name { get; } = "dump-xml";

        public string Syntax => $"{Name} <file>";

        public string Description =>
            "Dump data center contents to a specified directory as XML.";

        public int RequiredArguments { get; } = 1;

        public void Run(string output, string[] args)
        {
            if (output == null)
                output = "Xml";

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
                            var settings = new XmlWriterSettings
                            {
                                Indent = true
                            };

                            using (var writer = XmlWriter.Create(
                                Path.Combine(dir, $"{grp.Key}-{i}.xml"), settings))
                                WriteElement(writer, elem);
                        }

                        i++;
                    }
                }
            }
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
