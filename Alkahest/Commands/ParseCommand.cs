using Alkahest.Core;
using Alkahest.Core.Logging;
using Alkahest.Core.Net.Game;
using Alkahest.Core.Net.Game.Logging;
using Alkahest.Core.Net.Game.Serialization;
using Alkahest.Parser;
using Mono.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Alkahest.Commands
{
    sealed class ParseCommand : AlkahestCommand
    {
        const string ParsedExtension = "txt";

        const RegexOptions FilterRegexOptions =
            RegexOptions.Compiled |
            RegexOptions.Singleline;

        static readonly Log _log = new Log(typeof(ParseCommand));

        string _output;

        bool _header;

        bool _stats;

        bool _summary;

        readonly List<Regex> _regexes = new List<Regex>();

        HexDumpMode _hex = HexDumpMode.Unknown;

        bool _parse = true;

        PacketSerializerBackend _backend = PacketSerializerBackend.Reflection;

        int _roundtrips;

        AnalysisMode _analysis = AnalysisMode.None;

        int _length;

        bool _whiteSpace;

        bool _control;

        public ParseCommand()
            : base("Parser", "parse", "Parse a packet log file")
        {
            Options = new OptionSet
            {
                $"Usage: {Program.Name} {Name} [OPTIONS]",
                string.Empty,
                "Available options:",
                string.Empty,
                {
                    "o|output",
                    $"Specify output file (defaults to input file name with extension changed to `{ParsedExtension}`)",
                    o => _output = o
                },
                {
                    "r|regex=",
                    "Add an opcode regex to filter packets by (uses Perl 5 regex syntax)",
                    (string r) => _regexes.Add(new Regex(r, FilterRegexOptions))
                },
                {
                    "e|header",
                    $"Output packet log header information before parsing (defaults to `{_header}`)",
                    e => _header = e != null
                },
                {
                    "s|stats",
                    $"Output parsing and analysis statistics before exiting (defaults to `{_stats}`)",
                    s => _stats = s != null
                },
                {
                    "u|summary",
                    $"Output a summary of known and unknown packets before exiting (defaults to `{_summary}`)",
                    u => _summary = u != null
                },
                {
                    "x|hex-dump=",
                    $"Specify hex dump mode (defaults to `{_hex}`)",
                    (HexDumpMode x) => _hex = x
                },
                {
                    "p|parse",
                    $"Enable/disable parsing of known packets (defaults to `{_parse}`)",
                    p => _parse = p != null
                },
                {
                    "z|backend=",
                    $"Specify packet serializer backend (defaults to `{_backend}`)",
                    (PacketSerializerBackend z) => _backend = z
                },
                {
                    "t|roundtrips=",
                    $"Specify the number of times to roundtrip parsed packets (defaults to `{_roundtrips}`)",
                    (int t) => _roundtrips = t
                },
                {
                    "a|analyze=",
                    $"Specify analysis mode (defaults to `{_analysis}`)",
                    (AnalysisMode a) => _analysis = a
                },
                {
                    "m|min-string-length=",
                    $"Specify minimum string length for analysis (defaults to `{_length}`)",
                    (int m) => _length = m
                },
                {
                    "w|allow-white-space",
                    $"Enable/disable showing strings consisting only of white space (defaults to `{_whiteSpace}`)",
                    w => _whiteSpace = w != null
                },
                {
                    "c|allow-control-chars",
                    $"Enable/disable showing strings containing control characters (defaults to `{_control}`)",
                    c => _control = c != null
                },
                string.Empty,
                "Hex dump modes:",
                string.Empty,
                $"  {HexDumpMode.None}: Don't do hex dumps",
                $"  {HexDumpMode.Unknown}: Do hex dumps for packets with unknown structure (default)",
                $"  {HexDumpMode.All}: Do hex dumps for all packets",
                string.Empty,
                "Packet serializer backends:",
                string.Empty,
                $"  {PacketSerializerBackend.Reflection}: Reflection-based serializer (default)",
                $"  {PacketSerializerBackend.Compiler}: Runtime-compiled serializer",
                string.Empty,
                "Analysis modes:",
                string.Empty,
                $"  {AnalysisMode.None}: Don't perform analysis (default)",
                $"  {AnalysisMode.Unknown}: Analyze packets with unknown structure",
                $"  {AnalysisMode.All}: Analyze all packets",
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

            if (_output == null)
                _output = Path.ChangeExtension(input, ParsedExtension);

            if (_regexes.Count == 0)
                _regexes.Add(new Regex(".*", FilterRegexOptions));

            _log.Basic("Parsing {0}...", input);

            var stats = new PacketStatistics();
            using var reader = new PacketLogReader(input);

            if (_header)
            {
                _log.Info(string.Empty);
                _log.Info("Version: {0}", reader.Version);
                _log.Info("Compressed: {0}", reader.IsCompressed);
                _log.Info("Region: {0}", reader.Region);
                _log.Info("Servers:");

                foreach (var srv in reader.Servers.Values)
                    _log.Info("  {0} ({1}): {2} -> {3}", srv.Name, srv.Id, srv.RealEndPoint,
                        srv.ProxyEndPoint);

                _log.Info(string.Empty);
            }

            PacketSerializer serializer;

            switch (_backend)
            {
                case PacketSerializerBackend.Reflection:
                    serializer = new ReflectionPacketSerializer(reader.Region, reader.GameMessages,
                        reader.SystemMessages);
                    break;
                case PacketSerializerBackend.Compiler:
                    serializer = new CompilerPacketSerializer(reader.Region, reader.GameMessages,
                        reader.SystemMessages);
                    break;
                default:
                    throw Assert.Unreachable();
            }

            using var result = new StreamWriter(new FileStream(_output, FileMode.Create, FileAccess.Write));

            foreach (var entry in reader.EnumerateAll())
                HandleEntry(reader, entry, stats, serializer, result);

            _log.Basic("Parsed packets to {0}", _output);

            PrintStatistics(stats);

            return 0;
        }

        void HandleEntry(PacketLogReader reader, PacketLogEntry entry, PacketStatistics stats,
            PacketSerializer serializer, StreamWriter result)
        {
            stats.TotalPackets++;

            var name = serializer.GameMessages.CodeToName[entry.OpCode];

            if (_regexes.All(r => !r.IsMatch(name)))
            {
                stats.IgnoredPackets++;
                return;
            }

            result.WriteLine("[{0:yyyy-MM-dd HH:mm:ss:fff}] {1} {2}: {3} ({4} bytes)",
                entry.Timestamp.ToLocalTime(), reader.Servers[entry.ServerId].Name,
                entry.Direction.ToDirectionString(), name, entry.Payload.Count);

            var parsed = serializer.Create(entry.OpCode);
            var payload = entry.Payload.ToArray();

            if (payload.Length != 0)
            {
                if ((_hex == HexDumpMode.Unknown && parsed == null) || _hex == HexDumpMode.All)
                {
                    result.WriteLine();
                    result.WriteLine(new RawPacket(name)
                    {
                        Payload = payload,
                    });
                }

                if ((_analysis == AnalysisMode.Unknown && parsed == null) || _analysis == AnalysisMode.All)
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

                    var strings = PacketAnalysis.FindStrings(payload, _whiteSpace, _control, _length);

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

            if (parsed != null && _parse)
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

            result.WriteLine();
        }

        void PrintStatistics(PacketStatistics stats)
        {
            if (_stats)
            {
                static void PrintValue(string name, int value, string trail = "")
                {
                    _log.Info("{0,17}: {1}{2}", name, value, trail);
                }

                static void PrintPercentageValue(string name, int value, int total)
                {
                    PrintValue(name, value, total == 0 ? string.Empty : $" ({(double)value / total:P2})");
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
                void PrintSummary(KeyValuePair<string, PacketStatistics.SummaryEntry> kvp)
                {
                    var entry = kvp.Value;
                    var total = stats.RelevantPackets;
                    var sizes = entry.Sizes;

                    _log.Info("  {0}", kvp.Key);
                    _log.Info("    Count: {0}{1}", entry.Count, total != 0 ?
                        $" ({(double)entry.Count / total:P2})" : string.Empty);
                    _log.Info("    Sizes: Min = {0}, Max = {1}, Avg = {2}", sizes.Min(), sizes.Max(),
                        (int)sizes.Average());
                    _log.Info(string.Empty);
                }

                void PrintSummaryList(string header, bool known)
                {
                    var packets = stats.Packets.Where(x => x.Value.IsKnown == known).OrderBy(x => x.Key);

                    if (!packets.Any())
                        return;

                    _log.Info(string.Empty);
                    _log.Info($"{header}:");
                    _log.Info(string.Empty);

                    foreach (var kvp in packets)
                        PrintSummary(kvp);
                }

                PrintSummaryList("Known packets", true);
                PrintSummaryList("Unknown packets", false);
            }
        }
    }
}
