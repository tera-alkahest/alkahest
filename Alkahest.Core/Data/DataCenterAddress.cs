using System;
using System.Runtime.InteropServices;

namespace Alkahest.Core.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct DataCenterAddress : IEquatable<DataCenterAddress>
    {
        public static readonly DataCenterAddress Zero = new DataCenterAddress();

        public readonly ushort SegmentIndex;

        public readonly ushort ElementIndex;

        public DataCenterAddress(ushort segmentIndex, ushort elementIndex)
        {
            SegmentIndex = segmentIndex;
            ElementIndex = elementIndex;
        }

        public bool Equals(DataCenterAddress other)
        {
            return SegmentIndex == other.SegmentIndex &&
                ElementIndex == other.ElementIndex;
        }

        public override bool Equals(object obj)
        {
            return obj is DataCenterAddress a ? Equals(a) : false;
        }

        public override int GetHashCode()
        {
            return (Bits.Insert(0, SegmentIndex, 0, 15) |
                Bits.Insert(0, ElementIndex, 16, 31)).GetHashCode();
        }
    }
}
