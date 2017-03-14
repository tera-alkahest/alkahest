using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security;
using Alkahest.Core;
using Alkahest.Core.Logging;
using Alkahest.Core.Logging.Loggers;
using Alkahest.Core.Net.Protocol;
using Alkahest.Core.Net.Protocol.Logging;

namespace Alkahest.Parser
{
    static class Application
    {
        public static string Name { get; } =
            $"{nameof(Alkahest)} {nameof(Parser)}";

        static readonly Log _log = new Log(typeof(Application));

        public static int Run(string[] args)
        {
            Assert.Enabled = true;

            Log.Level = LogLevel.Info;
            Log.TimestampFormat = "HH:mm:ss:fff";

            var color = Console.ForegroundColor;

            Log.Loggers.Add(new ConsoleLogger(false,
                color, color, color, color));

            if (!Debugger.IsAttached)
                AppDomain.CurrentDomain.UnhandledException += UnhandledException;

            if (args.Length != 1 && args.Length != 2)
            {
                _log.Error("Expected input file name and optional output file name");
                return 1;
            }

            var inputName = args[0];
            var resultName = args.Length >= 2 ?
                args[1] : Path.ChangeExtension(inputName, "txt");

            _log.Basic("Parsing {0} to {1}...", inputName, resultName);

            var reader = new PacketLogReader(inputName);
            var serializer = new PacketSerializer(
                new OpCodeTable(true, reader.Region),
                new OpCodeTable(false, reader.Region));
            var count = 0;
            var known = 0;

            PacketLogEntry entry;

            using (var result = new StreamWriter(new FileStream(resultName,
                FileMode.Create, FileAccess.Write)))
            {
                while ((entry = reader.Read()) != null)
                {
                    count++;

                    result.WriteLine("[{0:yyyy-MM-dd HH:mm:ss:fff}] {1} {2}: {3} ({4} bytes)",
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
                        known++;

                        serializer.Deserialize(raw.Payload, parsed);

                        result.WriteLine();
                        result.WriteLine(parsed);
                    }

                    result.WriteLine();
                }
            }

            _log.Basic("Parsed {0} packets ({1:P2} with known structure)",
                count, (double)known / count);

            return 0;
        }

        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        static void UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            _log.Error("Unhandled exception:");
            _log.Error(args.ExceptionObject.ToString());
            _log.Error("{0} will terminate", Name);
        }
    }
}
