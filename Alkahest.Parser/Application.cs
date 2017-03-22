using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Text.RegularExpressions;
using Mono.Options;
using Alkahest.Core;
using Alkahest.Core.Logging;
using Alkahest.Core.Logging.Loggers;
using Alkahest.Core.Net.Protocol;
using Alkahest.Core.Net.Protocol.Logging;
using Alkahest.Parser.Analysis;

namespace Alkahest.Parser
{
    static class Application
    {
        const RegexOptions RegexOptions =
            System.Text.RegularExpressions.RegexOptions.Compiled |
            System.Text.RegularExpressions.RegexOptions.Singleline;

        public static string Name { get; } =
            $"{nameof(Alkahest)} {nameof(Parser)}";

        static readonly Log _log = new Log(typeof(Application));

        static string _output;

        static bool _stats;

        static List<string> _regexes = new List<string>();

        static HexDumpMode _hex = HexDumpMode.Unknown;

        static bool _parse = true;

        static int _roundtrips;

        static AnalysisMode _analysis;

        static int _length;

        static bool _whiteSpace;

        static bool _control;

        static void ShowVersion()
        {
            var asmName = Assembly.GetExecutingAssembly().GetName();

            Console.WriteLine("{0} {1}", asmName.Name, asmName.Version);
        }

        static void ShowHelp(OptionSet set)
        {
            var name = Assembly.GetExecutingAssembly().GetName().Name;

            Console.WriteLine("This is {0}, part of the {1} project.",
                name, nameof(Alkahest));
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine();
            Console.WriteLine("  {0} [options...] [--] <file>", name);
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine();

            set.WriteOptionDescriptions(Console.Out);

            Console.WriteLine();
            Console.WriteLine("Hex dump modes:");
            Console.WriteLine();
            Console.WriteLine("  {0}: Don't do hex dumps.",
                HexDumpMode.None);
            Console.WriteLine("  {0}: Do hex dumps for packets with unknown structure.",
                HexDumpMode.Unknown);
            Console.WriteLine("  {0}: Do hex dumps for all packets.",
                HexDumpMode.All);
            Console.WriteLine();
            Console.WriteLine("Analysis modes:");
            Console.WriteLine();
            Console.WriteLine("  {0}: Don't perform analysis.",
                AnalysisMode.None);
            Console.WriteLine("  {0}: Analyze packets with unknown structure.",
                AnalysisMode.Unknown);
            Console.WriteLine("  {0}: Analyze all packets.",
                AnalysisMode.All);
        }

        static bool HandleArguments(ref string[] args)
        {
            var version = false;
            var help = false;
            var set = new OptionSet
            {
                "General",
                {
                    "h|?|help",
                    "Print version and exit.",
                    h => help = h != null
                },
                {
                    "v|version",
                    "Print help and exit.",
                    v => version = v != null
                },
                {
                    "o|output",
                    "Specify output text file.",
                    o => _output = o
                },
                {
                    "r|regex=",
                    "Add an opcode regex to filter packets by. Uses Perl 5 regex syntax.",
                    (string ar) => _regexes.Add(ar)
                },
                {
                    "s|stats",
                    "Output parsing and analysis statistics before exiting.",
                    s => _stats = s != null
                },
                "Parsing",
                {
                    "x|hex-dump=",
                    "Specify hex dump mode.",
                    (HexDumpMode x) => _hex = x
                },
                {
                    "p|parse",
                    "Enable/disable parsing of known packets.",
                    p => _parse = p != null
                },
                {
                    "t|roundtrips=",
                    "Specify the number of times to roundtrip parsed packets.",
                    (int t) => _roundtrips = t
                },
                "Analysis",
                {
                    "a|analyze=",
                    "Specify analysis mode.",
                    (AnalysisMode a) => _analysis = a
                },
                {
                    "m|min-string-length",
                    "Specify minimum string length.",
                    (int m) => _length = m
                },
                {
                    "w|allow-white-space",
                    "Enable/disable showing strings consisting only of white space.",
                    w => _whiteSpace = w != null
                },
                {
                    "c|allow-control-chars",
                    "Enable/disable showing strings containing control characters.",
                    c => _control = c != null
                }
            };

            args = set.Parse(args).ToArray();

            if (version)
            {
                ShowVersion();
                return false;
            }

            if (help)
            {
                ShowHelp(set);
                return false;
            }

            return true;
        }

        public static int Run(string[] args)
        {
            try
            {
                if (!HandleArguments(ref args))
                    return 0;
            }
            catch (OptionException e)
            {
                Console.WriteLine(e.Message);
                return 1;
            }

            if (args.Length != 1)
            {
                Console.WriteLine("Expected exactly one input file argument.");
                return 1;
            }

            Log.Level = LogLevel.Debug;
            Log.TimestampFormat = "HH:mm:ss:fff";

            var color = Console.ForegroundColor;

            Log.Loggers.Add(new ConsoleLogger(false,
                color, color, color, color));

            if (!Debugger.IsAttached)
                AppDomain.CurrentDomain.UnhandledException += UnhandledException;

            var input = args[0];
            var output = _output ?? Path.ChangeExtension(input, "txt");
            var regexes = _regexes.Select(x => new Regex(x, RegexOptions))
                .DefaultIfEmpty(new Regex(".*", RegexOptions))
                .ToArray();

            _log.Basic("Parsing {0}...", input);

            var reader = new PacketLogReader(input);
            var serializer = new PacketSerializer(
                new OpCodeTable(true, reader.Region),
                new OpCodeTable(false, reader.Region));
            var stats = new PacketStatistics();

            using (var result = new StreamWriter(new FileStream(output,
                FileMode.Create, FileAccess.Write)))
            {
                foreach (var entry in reader.EnumerateAll())
                {
                    stats.TotalPackets++;

                    var name = serializer.GameMessages.OpCodeToName[entry.OpCode];

                    if (regexes.All(r => !r.IsMatch(name)))
                    {
                        stats.IgnoredPackets++;

                        continue;
                    }

                    stats.RelevantPackets++;

                    result.WriteLine("[{0:yyyy-MM-dd HH:mm:ss:fff}] {1} {2}: {3} ({4} bytes)",
                        entry.Timestamp.ToLocalTime(), entry.ServerName,
                        entry.Direction.ToDirectionString(), name,
                        entry.Payload.Count);

                    var parsed = serializer.Create(entry.OpCode);
                    var payload = entry.Payload.ToArray();

                    if (payload.Length != 0)
                    {
                        if ((_hex == HexDumpMode.Unknown && parsed == null) ||
                            _hex == HexDumpMode.All)
                        {
                            result.WriteLine();
                            result.WriteLine(new RawPacket(name)
                            {
                                Payload = payload
                            });
                        }

                        if ((_analysis == AnalysisMode.Unknown && parsed == null) ||
                            _analysis == AnalysisMode.All)
                        {
                            var arrays = PacketAnalysis.FindArrays(payload);

                            if (arrays.Any())
                            {
                                result.WriteLine();
                                result.WriteLine("Potential arrays:");
                                result.WriteLine();

                                foreach (var arr in arrays)
                                {
                                    stats.PotentialArrays++;

                                    result.WriteLine(arr);
                                }
                            }

                            var strings = PacketAnalysis.FindStrings(payload,
                                _whiteSpace, _control, _length);

                            if (strings.Any())
                            {
                                result.WriteLine();
                                result.WriteLine("Potential strings:");
                                result.WriteLine();

                                foreach (var str in strings)
                                {
                                    stats.PotentialStrings++;

                                    result.WriteLine(str);
                                }
                            }
                        }
                    }
                    else
                        stats.EmptyPackets++;

                    if (parsed != null)
                    {
                        stats.KnownPackets++;

                        if (_parse)
                        {
                            stats.ParsedPackets++;

                            for (var i = 0; i < _roundtrips + 1; i++)
                            {
                                serializer.Deserialize(payload, parsed);

                                var payload2 = serializer.Serialize(parsed);

                                Assert.Check(payload2.Length == payload.Length,
                                    "Payload lengths must match after roundtrip.");

                                if (i > 0)
                                    Assert.Check(payload2.SequenceEqual(payload),
                                        "Payloads must match after first roundtrip.");

                                payload = payload2;
                            }

                            result.WriteLine();
                            result.WriteLine(parsed);
                        }
                    }
                    else
                        stats.UnknownPackets++;

                    result.WriteLine();
                }
            }

            _log.Basic("Parsed packets to {0}", output);

            if (_stats)
            {
                void PrintValue(string name, int value)
                {
                    _log.Info("{0,17}: {1}", name, value);
                }

                void PrintPacketValue(string name, int value)
                {
                    _log.Info("{0,17}: {1} ({2:P2})", name, value,
                        (double)value / stats.RelevantPackets);
                }

                PrintValue("Total packets", stats.TotalPackets);
                PrintPacketValue("Relevant packets", stats.RelevantPackets);
                PrintPacketValue("Ignored packets", stats.IgnoredPackets);
                PrintPacketValue("Empty packets", stats.EmptyPackets);
                PrintPacketValue("Unknown packets", stats.UnknownPackets);
                PrintPacketValue("Known packets", stats.KnownPackets);
                PrintPacketValue("Parsed packets", stats.ParsedPackets);
                PrintValue("Potential arrays", stats.PotentialArrays);
                PrintValue("Potential strings", stats.PotentialStrings);
            }

            return 0;
        }

        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        static void UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            _log.Error("Unhandled exception:");
            _log.Error(args.ExceptionObject.ToString());
            _log.Error("{0} will terminate", Name);

            Environment.Exit(1);
        }
    }
}
