using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alkahest.Analyzer.Analysis
{
    sealed class PotentialArray
    {
        public int CountPosition { get; }

        public ushort Count { get; }

        public int OffsetPosition { get; }

        public ushort Offset { get; }

        public IReadOnlyList<PotentialArrayElement> Elements { get; }

        public PotentialArray(int countPosition, ushort count,
            int offsetPosition, ushort offset,
            IEnumerable<PotentialArrayElement> elements)
        {
            CountPosition = countPosition;
            Count = count;
            OffsetPosition = offsetPosition;
            Offset = offset;
            Elements = elements.ToArray();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"{CountPosition:X4} => {Count} | " +
                $"{OffsetPosition:X4} => {Offset:X4}");
            sb.AppendLine($"{{");

            foreach (var elem in Elements)
                sb.AppendLine($"    {elem}");

            sb.Append("}");

            return sb.ToString();
        }
    }
}
