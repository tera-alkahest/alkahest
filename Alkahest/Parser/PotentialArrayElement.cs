namespace Alkahest.Parser
{
    sealed class PotentialArrayElement
    {
        public int HerePosition { get; }

        public int Here { get; }

        public int NextPosition { get; }

        public int Next { get; }

        public PotentialArrayElement(int here, int nextPosition, int next)
        {
            HerePosition = Here = here;
            NextPosition = nextPosition;
            Next = next;
        }

        public override string ToString()
        {
            var delta = Next == 0 ?
                string.Empty : $" (delta = {Next - NextPosition - sizeof(ushort)} bytes)";

            return $"{Here:X4} ... {Next:X4}{delta}";
        }
    }
}
