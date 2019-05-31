using Alkahest.Core.IO;
using System.Collections.Generic;
using System.IO;

namespace Alkahest.Core.Data
{
    sealed class DataCenterSegmentedRegion
    {
        public uint ElementSize { get; }

        public IReadOnlyList<DataCenterSegment> Segments { get; }

        public DataCenterSegmentedRegion(uint elementSize, IReadOnlyList<DataCenterSegment> segments)
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
