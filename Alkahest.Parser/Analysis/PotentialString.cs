namespace Alkahest.Parser.Analysis
{
    sealed class PotentialString
    {
        public int OffsetPosition { get; }

        public int Offset { get; }

        public string Value { get; }

        public PotentialString(int offsetPosition, int offset, string value)
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
