using System.Threading;
using Alkahest.Core.IO;

namespace Alkahest.Core.Data
{
    sealed class DataCenterSegment
    {
        public uint ElementSize { get; }

        public uint FullCount { get; }

        public uint UsedCount { get; }

        public byte[] Data { get; }

        readonly ThreadLocal<TeraBinaryReader> _reader =
            new ThreadLocal<TeraBinaryReader>();

        public DataCenterSegment(uint elementSize, uint fullCount,
            uint usedCount, byte[] data)
        {
            ElementSize = elementSize;
            FullCount = fullCount;
            UsedCount = usedCount;
            Data = data;
        }

        public TeraBinaryReader GetReader(uint elementIndex)
        {
            var reader = _reader.Value ??
                (_reader.Value = new TeraBinaryReader(Data));

            reader.Position = (int)(elementIndex * ElementSize);

            return reader;
        }
    }
}
