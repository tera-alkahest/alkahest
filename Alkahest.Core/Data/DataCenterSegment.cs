using Alkahest.Core.IO;
using System.Threading;

namespace Alkahest.Core.Data
{
    sealed class DataCenterSegment
    {
        public uint ElementSize { get; }

        public uint FullCount { get; }

        public uint UsedCount { get; }

        public byte[] Data { get; }

        readonly ThreadLocal<GameBinaryReader> _reader = new ThreadLocal<GameBinaryReader>();

        public DataCenterSegment(uint elementSize, uint fullCount, uint usedCount, byte[] data)
        {
            ElementSize = elementSize;
            FullCount = fullCount;
            UsedCount = usedCount;
            Data = data;
        }

        public GameBinaryReader GetReader(uint elementIndex)
        {
            var reader = _reader.Value ??= new GameBinaryReader(Data);

            reader.Position = (int)(elementIndex * ElementSize);

            return reader;
        }
    }
}
