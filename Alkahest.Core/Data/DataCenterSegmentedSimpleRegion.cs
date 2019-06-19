using Alkahest.Core.IO;
using System.Collections.Generic;
using System.IO;

namespace Alkahest.Core.Data
{
    sealed class DataCenterSegmentedSimpleRegion
    {
        public uint ElementSize { get; }

        public IReadOnlyList<DataCenterSimpleRegion> Segments { get; }

        public DataCenterSegmentedSimpleRegion(uint elementSize,
            IReadOnlyList<DataCenterSimpleRegion> segments)
        {
            ElementSize = elementSize;
            Segments = segments;
        }

        public GameBinaryReader GetReader(DataCenterAddress address)
        {
            if (address.SegmentIndex >= Segments.Count)
                throw new InvalidDataException();

            return Segments[address.SegmentIndex].GetReader(address.ElementIndex);
        }
    }
}
