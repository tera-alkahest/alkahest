using Alkahest.Core.IO;
using System.IO;
using System.Threading;

namespace Alkahest.Core.Data
{
    sealed class DataCenterRegion
    {
        public uint ElementSize { get; }

        public uint FullCount { get; }

        public uint UsedCount { get; }

        public byte[] Data { get; }

        readonly ThreadLocal<GameBinaryReader> _reader = new ThreadLocal<GameBinaryReader>();

        public DataCenterRegion(uint elementSize, uint fullCount, uint usedCount, byte[] data)
        {
            if (usedCount > fullCount)
                throw new InvalidDataException(
                    $"Used count {usedCount} is greater than full count {fullCount}.");

            ElementSize = elementSize;
            FullCount = fullCount;
            UsedCount = usedCount;
            Data = data;
        }

        public GameBinaryReader GetReader(uint elementIndex)
        {
            if (elementIndex >= UsedCount)
                throw new InvalidDataException($"Element index {elementIndex} is greater than {UsedCount}.");

            var reader = _reader.Value ??= new GameBinaryReader(Data);

            reader.Position = (int)(elementIndex * ElementSize);

            return reader;
        }
    }
}
