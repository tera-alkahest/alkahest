using System;
using System.Runtime.InteropServices;

namespace Alkahest.Core.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    readonly struct DataCenterAddress : IEquatable<DataCenterAddress>
    {
        public static DataCenterAddress Zero => default;

        public ushort SegmentIndex { get; }

        public ushort ElementIndex { get; }

        public DataCenterAddress(ushort segmentIndex, ushort elementIndex)
        {
            SegmentIndex = segmentIndex;
            ElementIndex = elementIndex;
        }

        public bool Equals(DataCenterAddress other)
        {
            return SegmentIndex == other.SegmentIndex && ElementIndex == other.ElementIndex;
        }

        public override bool Equals(object obj)
        {
            return obj is DataCenterAddress a ? Equals(a) : false;
        }

        public override int GetHashCode()
        {
            return Bits.Compose(
                ((uint)SegmentIndex, 0, 16),
                (ElementIndex, 16, 16)).GetHashCode();
        }

        public static bool operator ==(DataCenterAddress a, DataCenterAddress b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(DataCenterAddress a, DataCenterAddress b)
        {
            return !a.Equals(b);
        }

        public override string ToString()
        {
            return $"{SegmentIndex}:{ElementIndex}";
        }
    }
}
