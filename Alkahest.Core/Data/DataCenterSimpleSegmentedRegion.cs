using System.Collections.Generic;
using System.Linq;

namespace Alkahest.Core.Data
{
    sealed class DataCenterSimpleSegmentedRegion
    {
        public uint ElementSize { get; }

        public IReadOnlyDictionary<ushort, DataCenterSimpleRegion> Segments { get; }

        public DataCenterSimpleSegmentedRegion(uint elementSize,
            IReadOnlyList<DataCenterSimpleRegion> segments)
        {
            ElementSize = elementSize;

            ushort i = 0;

            Segments = segments.ToDictionary(x => i++);
        }
    }
}
