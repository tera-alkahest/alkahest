using System;
using System.Collections.Generic;

namespace Alkahest.Parser
{
    sealed class PacketStatistics
    {
        public sealed class SummaryEntry
        {
            public int Count { get; set; }

            public List<int> Sizes { get; } = new List<int>();
        }

        public int TotalPackets { get; set; }

        public int RelevantPackets { get; set; }

        public int IgnoredPackets { get; set; }

        public int EmptyPackets { get; set; }

        public int ParsedPackets { get; set; }

        public int PotentialArrays { get; set; }

        public int PotentialStrings { get; set; }

        public IReadOnlyDictionary<string, SummaryEntry> KnownPackets =>
            _known;

        public IReadOnlyDictionary<string, SummaryEntry> UnknownPackets =>
            _unknown;

        readonly Dictionary<string, SummaryEntry> _known =
            new Dictionary<string, SummaryEntry>();

        readonly Dictionary<string, SummaryEntry> _unknown =
            new Dictionary<string, SummaryEntry>();

        public void AddPacket(string opCode, bool known, int length)
        {
            var dict = known ? _known : _unknown;

            if (!dict.TryGetValue(opCode, out var entry))
                dict.Add(opCode, entry = new SummaryEntry());

            entry.Count++;
            entry.Sizes.Add(length);
        }
    }
}
