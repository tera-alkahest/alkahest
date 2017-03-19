namespace Alkahest.Parser
{
    sealed class PacketStatistics
    {
        public int TotalPackets { get; set; }

        public int RelevantPackets { get; set; }

        public int IgnoredPackets { get; set; }

        public int EmptyPackets { get; set; }

        public int UnknownPackets { get; set; }

        public int KnownPackets { get; set; }

        public int ParsedPackets { get; set; }

        public int PotentialArrays { get; set; }

        public int PotentialStrings { get; set; }
    }
}
