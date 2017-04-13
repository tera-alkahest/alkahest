using Alkahest.Core.IO;

namespace Alkahest.Core.Data
{
    sealed class DataCenterSimpleRegion
    {
        public uint ElementSize { get; }

        public uint Count { get; }

        public byte[] Data { get; }

        TeraBinaryReader _reader;

        public DataCenterSimpleRegion(uint elementSize, uint count,
            byte[] data)
        {
            ElementSize = elementSize;
            Count = count;
            Data = data;
        }

        public TeraBinaryReader GetReader(uint elementIndex)
        {
            var reader = _reader ?? (_reader = new TeraBinaryReader(Data));

            reader.Position = (int)(elementIndex * ElementSize);

            return reader;
        }
    }
}
