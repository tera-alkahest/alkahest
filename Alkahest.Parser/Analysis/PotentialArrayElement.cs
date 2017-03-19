namespace Alkahest.Parser.Analysis
{
    sealed class PotentialArrayElement
    {
        public int HerePosition { get; }

        public ushort Here { get; }

        public int NextPosition { get; }

        public ushort Next { get; }

        public PotentialArrayElement(ushort here, int nextPosition,
            ushort next)
        {
            HerePosition = Here = here;
            NextPosition = nextPosition;
            Next = next;
        }

        public override string ToString()
        {
            var delta = Next == 0 ?
                string.Empty : $" (ƒ¢ = {Next - NextPosition - sizeof(ushort)} bytes)";

            return $"{Here:X4} ... {Next:X4}{delta}";
        }
    }
}
