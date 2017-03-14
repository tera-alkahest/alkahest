using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security;
using Alkahest.Analyzer.Analysis;
using Alkahest.Core;
using Alkahest.Core.Logging;
using Alkahest.Core.Logging.Loggers;
using Alkahest.Core.Net.Protocol;
using Alkahest.Core.Net.Protocol.Logging;

namespace Alkahest.Analyzer
{
    static class Application
    {
        public static string Name { get; } =
            $"{nameof(Alkahest)} {nameof(Analyzer)}";

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

            if (args.Length != 2 && args.Length != 3)
            {
                _log.Error("Expected input file name, opcode name, and optional output file name");
                return 1;
            }

            var inputName = args[0];
            var resultName = args.Length >= 3 ?
                args[2] : Path.ChangeExtension(inputName, "txt");

            _log.Basic("Analyzing {0} to {1}...", inputName, resultName);

            var opCodeName = args[1];
            var reader = new PacketLogReader(inputName);
            var opc = new OpCodeTable(true, reader.Region);

            if (!opc.NameToOpCode.TryGetValue(opCodeName, out var opCode))
            {
                _log.Error("Invalid opcode {0}", opCodeName);
                return 1;
            }

            var serializer = new PacketSerializer(opc,
                new OpCodeTable(false, reader.Region));

            var packets = new List<Tuple<PacketLogEntry, RawPacket>>();

            PacketLogEntry e;

            while ((e = reader.Read()) != null)
            {
                if (e.OpCode != opCode)
                    continue;

                packets.Add(Tuple.Create(e, new RawPacket(opCodeName)
                {
                    Payload = e.Payload.ToArray()
                }));
            }

            _log.Basic("Found {0} packets with opcode {1}",
                packets.Count, opCodeName);

            var arrs = 0;
            var strs = 0;

            using (var result = new StreamWriter(new FileStream(resultName,
                FileMode.Create, FileAccess.Write)))
            {
                foreach (var tup in packets)
                {
                    var entry = tup.Item1;
                    var raw = tup.Item2;

                    result.WriteLine("[{0}:yyyy-MM-dd HH:mm:ss:fff] {1} {2}: {3} ({4} bytes)",
                        entry.Timestamp.ToLocalTime(), entry.ServerName,
                        entry.Direction.ToDirectionString(),
                        serializer.GameMessages.OpCodeToName[entry.OpCode],
                        entry.Payload.Count);

                    if (raw.Payload.Length != 0)
                    {
                        result.WriteLine();
                        result.WriteLine(raw);

                        var arrays = PacketAnalysis.FindArrays(raw.Payload);

                        if (arrays.Any())
                        {
                            result.WriteLine();
                            result.WriteLine("Potential arrays:");
                            result.WriteLine();

                            foreach (var a in arrays)
                            {
                                arrs++;

                                result.WriteLine(a);
                            }
                        }

                        var strings = PacketAnalysis.FindStrings(raw.Payload);

                        if (strings.Any())
                        {
                            result.WriteLine();
                            result.WriteLine("Potential strings:");
                            result.WriteLine();

                            foreach (var s in strings)
                            {
                                strs++;

                                result.WriteLine(s);
                            }
                        }
                    }

                    result.WriteLine();
                }
            }

            _log.Basic("Found {0} potential arrays and {1} potential strings",
                arrs, strs);

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
