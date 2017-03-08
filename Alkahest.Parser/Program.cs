using System;
using System.IO;
using System.Linq;
using Alkahest.Core;
using Alkahest.Core.Net.Protocol;
using Alkahest.Core.Net.Protocol.Logging;

namespace Alkahest.Parser
{
    static class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 1 && args.Length != 2)
            {
                Console.WriteLine("Error: Expected input file name and optional output file name");
                return 1;
            }

            var inputName = args[0];
            var resultName = args.Length >= 2 ?
                args[1] : Path.ChangeExtension(inputName, "txt");

            var reader = new PacketLogReader(inputName);
            var serializer = new PacketSerializer(
                new OpCodeTable(true, reader.Region),
                new OpCodeTable(false, reader.Region));
            var result = new StreamWriter(new FileStream(resultName,
                FileMode.Create, FileAccess.Write));

            PacketLogEntry entry;

            while ((entry = reader.Read()) != null)
            {
                result.WriteLine("[{0}] {1} {2}: {3} ({4} bytes)",
                    entry.Timestamp.ToLocalTime(), entry.ServerName,
                    entry.Direction.ToDirectionString(),
                    serializer.GameMessages.OpCodeToName[entry.OpCode],
                    entry.Payload.Count);

                var raw = new RawPacket(
                    serializer.GameMessages.OpCodeToName[entry.OpCode])
                {
                    Payload = entry.Payload.ToArray()
                };

                if (raw.Payload.Length != 0)
                {
                    result.WriteLine();
                    result.WriteLine(raw);
                }

                var parsed = serializer.Create(entry.OpCode);

                if (parsed != null)
                {
                    serializer.Deserialize(raw.Payload, parsed);

                    result.WriteLine();
                    result.WriteLine(parsed);
                }

                result.WriteLine();
            }

            return 0;
        }
    }
}
