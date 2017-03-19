namespace Alkahest.Parser.Analysis
{
    sealed class PotentialString
    {
        public int OffsetPosition { get; }

        public ushort Offset { get; }

        public string Value { get; }

        public PotentialString(int offsetPosition, ushort offset, string value)
        {
            OffsetPosition = offsetPosition;
            Offset = offset;
            Value = value;
        }

        public override string ToString()
        {
            return $"{OffsetPosition:X4} => {Offset:X4} ({Value.Length}) = \"{Value}\"";
        }
    }
}
