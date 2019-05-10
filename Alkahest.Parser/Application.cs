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
using Alkahest.Core.Net.Protocol.Serializers;
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

        static bool _header;

        static bool _stats;

        static bool _summary;

        static readonly List<string> _regexes = new List<string>();

        static HexDumpMode _hex = HexDumpMode.Unknown;

        static bool _parse = true;

        static PacketSerializerBackend _backend;

        static int _roundtrips;

        static AnalysisMode _analysis;

        static int _length;

        static bool _whiteSpace;

        static bool _control;

        static bool HandleArguments(ref string[] args)
        {
            var asm = Assembly.GetExecutingAssembly();
            var name = asm.GetName().Name;
            var version = false;
            var help = false;
            var set = new OptionSet
            {
                $"This is {name}, part of the {nameof(Alkahest)} project.",
                "",
                "Usage:",
                "",
                $"  {name} [options...] [--] <file>",
                "",
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
                    "e|header",
                    "Output packet log header information before parsing.",
                    e => _header = e != null
                },
                {
                    "s|stats",
                    "Output parsing and analysis statistics before exiting.",
                    s => _stats = s != null
                },
                {
                    "u|summary",
                    "Output a summary of known and unknown packets before exiting.",
                    u => _summary = u != null
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
                    "z|backend=",
                    "Specify packet serializer backend.",
                    (PacketSerializerBackend z) => _backend = z
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
                    "m|min-string-length=",
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
                },
                "",
                "Hex dump modes:",
                "",
                $"  {HexDumpMode.None}: Don't do hex dumps.",
                $"  {HexDumpMode.Unknown}: Do hex dumps for packets with unknown structure.",
                $"  {HexDumpMode.All}: Do hex dumps for all packets.",
                "",
                "Packet serializer backends:",
                "",
                $"  {PacketSerializerBackend.Reflection}: Reflection-based serializer (default).",
                $"  {PacketSerializerBackend.Compiler}: Runtime-compiled serializer (default).",
                "",
                "Analysis modes:",
                "",
                $"  {AnalysisMode.None}: Don't perform analysis (default).",
                $"  {AnalysisMode.Unknown}: Analyze packets with unknown structure.",
                $"  {AnalysisMode.All}: Analyze all packets."
            };

            args = set.Parse(args).ToArray();

            if (version)
            {
                Console.WriteLine("{0} {1}", name,
                    asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    .InformationalVersion);
                return false;
            }

            if (help)
            {
                set.WriteOptionDescriptions(Console.Out);
                return false;
            }

            return true;
        }

        static void HandleEntry(PacketLogReader reader, PacketLogEntry entry,
            Regex[] regexes, PacketStatistics stats,
            PacketSerializer serializer, StreamWriter result)
        {
            stats.TotalPackets++;

            var name = serializer.Messages.Game.OpCodeToName[entry.OpCode];

            if (regexes.All(r => !r.IsMatch(name)))
            {
                stats.IgnoredPackets++;
                return;
            }

            result.WriteLine("[{0:yyyy-MM-dd HH:mm:ss:fff}] {1} {2}: {3} ({4} bytes)",
                entry.Timestamp.ToLocalTime(),
                reader.Servers[entry.ServerId].Name,
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

            stats.AddPacket(name, parsed != null, payload.Length);

            if (parsed != null)
            {
                if (_parse)
                {
                    stats.ParsedPackets++;

                    serializer.Deserialize(payload, parsed);

                    for (var i = 0; i < _roundtrips; i++)
                    {
                        var payload2 = serializer.Serialize(parsed);
                        var len = payload.Length;
                        var len2 = payload2.Length;

                        Assert.Check(len2 == len,
                            $"Payload lengths for {name} don't match ({len2} versus {len}).");

                        if (i > 0)
                            Assert.Check(payload2.SequenceEqual(payload),
                                $"Payloads for {name} don't match after roundtrip.");

                        if (i != _roundtrips - 1)
                            serializer.Deserialize(payload2, parsed);

                        payload = payload2;
                    }

                    result.WriteLine();
                    result.WriteLine(parsed);
                }
            }

            result.WriteLine();
        }

        static void PrintStats(PacketStatistics stats)
        {
            if (_stats)
            {
                void PrintValue(string name, int value,
                    string trail = "")
                {
                    _log.Info("{0,17}: {1}{2}", name, value, trail);
                }

                void PrintPercentageValue(string name, int value, int total)
                {
                    PrintValue(name, value, total == 0 ? string.Empty :
                        $" ({(double)value / total:P2})");
                }

                void PrintTotalPacketValue(string name, int value)
                {
                    PrintPercentageValue(name, value, stats.TotalPackets);
                }

                void PrintRelevantPacketValue(string name, int value)
                {
                    PrintPercentageValue(name, value, stats.RelevantPackets);
                }

                _log.Info(string.Empty);
                PrintValue("Total packets", stats.TotalPackets);
                PrintTotalPacketValue("Relevant packets", stats.RelevantPackets);
                PrintTotalPacketValue("Ignored packets", stats.IgnoredPackets);
                PrintRelevantPacketValue("Empty packets", stats.EmptyPackets);
                PrintRelevantPacketValue("Unknown packets", stats.UnknownPackets);
                PrintRelevantPacketValue("Known packets", stats.KnownPackets);
                PrintRelevantPacketValue("Parsed packets", stats.ParsedPackets);
                PrintValue("Potential arrays", stats.PotentialArrays);
                PrintValue("Potential strings", stats.PotentialStrings);
                _log.Info(string.Empty);
            }

            if (_summary)
            {
                void PrintSummary(KeyValuePair<string,
                    PacketStatistics.SummaryEntry> kvp)
                {
                    var entry = kvp.Value;
                    var total = stats.RelevantPackets;
                    var sizes = entry.Sizes;

                    _log.Info("  {0}", kvp.Key);
                    _log.Info("    Count: {0}{1}", entry.Count, total != 0 ?
                        $" ({(double)entry.Count / total:P2})" : string.Empty);
                    _log.Info("    Sizes: Min = {0}, Max = {1}, Avg = {2}",
                        sizes.Min(), sizes.Max(), (int)sizes.Average());
                    _log.Info(string.Empty);
                }

                void PrintSummaryList(string header,
                    Func<PacketStatistics.SummaryEntry, bool> predicate)
                {
                    var packets = stats.Packets.Where(x => predicate(x.Value))
                        .OrderBy(x => x.Key);

                    if (!packets.Any())
                        return;

                    _log.Info(string.Empty);
                    _log.Info($"{header}:");
                    _log.Info(string.Empty);

                    foreach (var kvp in packets)
                        PrintSummary(kvp);
                }

                PrintSummaryList("Known packets", x => x.Known);
                PrintSummaryList("Unknown packets", x => !x.Known);
            }
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
                Console.Error.WriteLine(e.Message);
                return 1;
            }

            if (args.Length != 1)
            {
                Console.Error.WriteLine("Expected exactly one input file argument.");
                return 1;
            }

            Log.Level = LogLevel.Debug;
            Log.TimestampFormat = "HH:mm:ss:fff";

            var color = Console.ForegroundColor;

            Log.Loggers.Add(new ConsoleLogger(false,
                color, color, color, color, color));

            if (!Debugger.IsAttached)
                AppDomain.CurrentDomain.UnhandledException += UnhandledException;

            var input = args[0];
            var output = _output ?? Path.ChangeExtension(input, "txt");
            var regexes = _regexes.Select(x => new Regex(x, RegexOptions))
                .DefaultIfEmpty(new Regex(".*", RegexOptions))
                .ToArray();

            _log.Basic("Parsing {0}...", input);

            var stats = new PacketStatistics();

            using (var reader = new PacketLogReader(input))
            {
                if (_header)
                {
                    _log.Info(string.Empty);
                    _log.Info("Version: {0}", reader.Version);
                    _log.Info("Compressed: {0}", reader.Compressed);
                    _log.Info("Region: {0}", reader.Messages.Region);
                    _log.Info("Servers:");

                    foreach (var srv in reader.Servers.Values)
                        _log.Info("  {0} ({1}): {2} -> {3}", srv.Name, srv.Id,
                            srv.RealEndPoint, srv.ProxyEndPoint);

                    _log.Info(string.Empty);
                }

                PacketSerializer serializer;

                switch (_backend)
                {
                    case PacketSerializerBackend.Reflection:
                        serializer = new ReflectionPacketSerializer(
                            reader.Messages);
                        break;
                    case PacketSerializerBackend.Compiler:
                        serializer = new CompilerPacketSerializer(
                            reader.Messages);
                        break;
                    default:
                        throw Assert.Unreachable();
                }

                using (var result = new StreamWriter(new FileStream(output,
                    FileMode.Create, FileAccess.Write)))
                    foreach (var entry in reader.EnumerateAll())
                        HandleEntry(reader, entry, regexes, stats, serializer,
                            result);
            }

            _log.Basic("Parsed packets to {0}", output);

            PrintStats(stats);

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
