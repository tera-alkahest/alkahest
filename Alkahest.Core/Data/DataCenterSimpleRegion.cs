using Alkahest.Core.IO;

namespace Alkahest.Core.Data
{
    sealed class DataCenterSimpleRegion
    {
        public uint ElementSize { get; }

        public uint Count { get; }

        public byte[] Data { get; }

        GameBinaryReader _reader;

        public DataCenterSimpleRegion(uint elementSize, uint count, byte[] data)
        {
            ElementSize = elementSize;
            Count = count;
            Data = data;
        }

        public GameBinaryReader GetReader(uint elementIndex)
        {
            _reader ??= new GameBinaryReader(Data);

            _reader.Position = (int)(elementIndex * ElementSize);

            return _reader;
        }
    }
}
