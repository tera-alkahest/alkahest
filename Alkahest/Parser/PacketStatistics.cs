using System.Collections.Generic;

namespace Alkahest.Parser
{
    sealed class PacketStatistics
    {
        public sealed class SummaryEntry
        {
            public int Count { get; set; }

            public List<int> Sizes { get; } = new List<int>();

            public bool IsKnown { get; }

            public SummaryEntry(bool known)
            {
                IsKnown = known;
            }
        }

        public int TotalPackets { get; set; }

        public int RelevantPackets { get; private set; }

        public int IgnoredPackets { get; set; }

        public int EmptyPackets { get; private set; }

        public int UnknownPackets { get; private set; }

        public int KnownPackets { get; private set; }

        public int ParsedPackets { get; set; }

        public int PotentialArrays { get; set; }

        public int PotentialStrings { get; set; }

        public IReadOnlyDictionary<string, SummaryEntry> Packets => _packets;

        readonly Dictionary<string, SummaryEntry> _packets = new Dictionary<string, SummaryEntry>();

        public void AddPacket(string opCode, bool known, int length)
        {
            RelevantPackets++;

            if (length == 0)
                EmptyPackets++;

            if (known)
                KnownPackets++;
            else
                UnknownPackets++;

            if (!_packets.TryGetValue(opCode, out var entry))
                _packets.Add(opCode, entry = new SummaryEntry(known));

            entry.Count++;
            entry.Sizes.Add(length);
        }
    }
}
