using System.Collections.Generic;
using System.IO;
using System.Linq;
using Alkahest.Core.IO;

namespace Alkahest.Core.Data
{
    sealed class DataCenterSegmentedRegion
    {
        public uint ElementSize { get; }

        public IReadOnlyDictionary<ushort, DataCenterSegment> Segments { get; }

        public DataCenterSegmentedRegion(uint elementSize,
            IReadOnlyList<DataCenterSegment> segments)
        {
            ElementSize = elementSize;

            ushort i = 0;

            Segments = segments.ToDictionary(x => i++);
        }

        public TeraBinaryReader GetReader(DataCenterAddress address)
        {
            if (address.SegmentIndex >= Segments.Count)
                throw new InvalidDataException();

            return Segments[address.SegmentIndex].GetReader(address.ElementIndex);
        }
    }
}
