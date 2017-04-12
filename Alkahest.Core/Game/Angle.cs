using System;

namespace Alkahest.Core.Game
{
    public struct Angle : IEquatable<Angle>, IComparable<Angle>
    {
        public readonly short Raw;

        public double Radians => Raw * 2 * Math.PI / (ushort.MaxValue + 1);

        public double Degrees => Raw * 360.0 / (ushort.MaxValue + 1);

        public Angle(short raw)
        {
            Raw = raw;
        }

        public static Angle FromRadians(double value)
        {
            return new Angle((short)(value / (2 * Math.PI) * (ushort.MaxValue + 1)));
        }

        public static Angle FromDegrees(double value)
        {
            return new Angle((short)(value / 360 * (ushort.MaxValue + 1)));
        }

        public bool Equals(Angle other)
        {
            return Raw == other.Raw;
        }

        public int CompareTo(Angle other)
        {
            return Raw.CompareTo(other.Raw);
        }

        public override bool Equals(object obj)
        {
            return obj is Angle a ? Equals(a) : false;
        }

        public override int GetHashCode()
        {
            return Raw.GetHashCode();
        }

        public override string ToString()
        {
            return $"[Raw: {Raw}, Radians: {Radians}, Degrees: {Degrees}]";
        }

        public static bool operator ==(Angle a, Angle b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Angle a, Angle b)
        {
            return !a.Equals(b);
        }

        public static bool operator >(Angle a, Angle b)
        {
            return a.CompareTo(b) > 0;
        }

        public static bool operator <(Angle a, Angle b)
        {
            return a.CompareTo(b) < 0;
        }

        public static bool operator >=(Angle a, Angle b)
        {
            return a.CompareTo(b) >= 0;
        }

        public static bool operator <=(Angle a, Angle b)
        {
            return a.CompareTo(b) <= 0;
        }
    }
}
