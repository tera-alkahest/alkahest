using System.Collections.Generic;

namespace Alkahest.Core.Data
{
    sealed class DataCenterSimpleSegmentedRegion
    {
        public uint ElementSize { get; }

        public IReadOnlyList<DataCenterSimpleRegion> Segments { get; }

        public DataCenterSimpleSegmentedRegion(uint elementSize,
            IReadOnlyList<DataCenterSimpleRegion> segments)
        {
            ElementSize = elementSize;
            Segments = segments;
        }
    }
}
