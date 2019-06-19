using Alkahest.Core.IO;
using System.IO;
using System.Threading;

namespace Alkahest.Core.Data
{
    sealed class DataCenterSimpleRegion
    {
        public uint ElementSize { get; }

        public uint Count { get; }

        public byte[] Data { get; }

        readonly ThreadLocal<GameBinaryReader> _reader = new ThreadLocal<GameBinaryReader>();

        public DataCenterSimpleRegion(uint elementSize, uint count, byte[] data)
        {
            ElementSize = elementSize;
            Count = count;
            Data = data;
        }

        public GameBinaryReader GetReader(uint elementIndex)
        {
            if (elementIndex >= Count)
                throw new InvalidDataException($"Element index {elementIndex} is greater than {Count}.");

            var reader = _reader.Value ??= new GameBinaryReader(Data);

            reader.Position = (int)(elementIndex * ElementSize);

            return reader;
        }
    }
}
