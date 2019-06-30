using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Alkahest.Core.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DataCenterValue : IEquatable<DataCenterValue>, IEquatable<int>, IEquatable<float>,
        IEquatable<string>, IEquatable<bool>
    {
        readonly string _stringValue;

        readonly int _primitiveValue;

        public readonly DataCenterTypeCode TypeCode;

        public bool IsNull => TypeCode == DataCenterTypeCode.None;

        public bool IsInt32 => TypeCode == DataCenterTypeCode.Int32;

        public bool IsSingle => TypeCode == DataCenterTypeCode.Single;

        public bool IsString => TypeCode == DataCenterTypeCode.String;

        public bool IsBoolean => TypeCode == DataCenterTypeCode.Boolean;

        public object Value
        {
            get
            {
                switch (TypeCode)
                {
                    case DataCenterTypeCode.None:
                        throw new InvalidOperationException("Value is null.");
                    case DataCenterTypeCode.Int32:
                        return AsInt32;
                    case DataCenterTypeCode.Single:
                        return AsSingle;
                    case DataCenterTypeCode.String:
                        return AsString;
                    case DataCenterTypeCode.Boolean:
                        return AsBoolean;
                    default:
                        throw Assert.Unreachable();
                }
            }
        }

        public int AsInt32
        {
            get
            {
                if (TypeCode != DataCenterTypeCode.Int32)
                    throw new InvalidOperationException();

                return _primitiveValue;
            }
        }

        public long AsInt64 => AsInt32;

        public float AsSingle
        {
            get
            {
                if (TypeCode != DataCenterTypeCode.Single)
                    throw new InvalidOperationException();

                var value = _primitiveValue;

                return Unsafe.As<int, float>(ref value);
            }
        }

        public double AsDouble => AsSingle;

        public bool AsBoolean
        {
            get
            {
                if (TypeCode != DataCenterTypeCode.Boolean)
                    throw new InvalidOperationException();

                return _primitiveValue != 0;
            }
        }

        public string AsString
        {
            get
            {
                if (TypeCode != DataCenterTypeCode.String)
                    throw new InvalidOperationException();

                return _stringValue;
            }
        }

        internal DataCenterValue(DataCenterTypeCode typeCode, int primitiveValue, string stringValue)
        {
            TypeCode = typeCode;
            _primitiveValue = primitiveValue;
            _stringValue = stringValue;
        }

        public bool Equals(DataCenterValue other)
        {
            if (TypeCode != other.TypeCode)
                return false;

            switch (TypeCode)
            {
                case DataCenterTypeCode.None:
                    return true;
                case DataCenterTypeCode.Int32:
                    return AsInt32 == other.AsInt32;
                case DataCenterTypeCode.Single:
                    return AsSingle == other.AsSingle;
                case DataCenterTypeCode.String:
                    return AsString == other.AsString;
                case DataCenterTypeCode.Boolean:
                    return AsBoolean == other.AsBoolean;
                default:
                    throw Assert.Unreachable();
            }
        }

        public bool Equals(int other)
        {
            return TypeCode == DataCenterTypeCode.Int32 && AsInt32 == other;
        }

        public bool Equals(float other)
        {
            return TypeCode == DataCenterTypeCode.Single && AsSingle == other;
        }

        public bool Equals(string other)
        {
            return TypeCode == DataCenterTypeCode.String && AsString == other;
        }

        public bool Equals(bool other)
        {
            return TypeCode == DataCenterTypeCode.Boolean && AsBoolean == other;
        }

        public int ToInt32(int? fallback = null)
        {
            switch (TypeCode)
            {
                case DataCenterTypeCode.None:
                case DataCenterTypeCode.Boolean:
                    throw new InvalidCastException();
                case DataCenterTypeCode.Int32:
                    return AsInt32;
                case DataCenterTypeCode.Single:
                    return (int)AsSingle;
                case DataCenterTypeCode.String:
                    return int.TryParse(AsString, NumberStyles.Integer, CultureInfo.InvariantCulture,
                        out var i) ? i : fallback ?? throw new InvalidCastException();
                default:
                    throw Assert.Unreachable();
            }
        }

        public long ToInt64(long? fallback = null)
        {
            switch (TypeCode)
            {
                case DataCenterTypeCode.None:
                case DataCenterTypeCode.Boolean:
                    throw new InvalidCastException();
                case DataCenterTypeCode.Int32:
                    return AsInt32;
                case DataCenterTypeCode.Single:
                    return (long)AsSingle;
                case DataCenterTypeCode.String:
                    return long.TryParse(AsString, NumberStyles.Integer, CultureInfo.InvariantCulture,
                        out var l) ? l : fallback ?? throw new InvalidCastException();
                default:
                    throw Assert.Unreachable();
            }
        }

        public float ToSingle(float? fallback = null)
        {
            switch (TypeCode)
            {
                case DataCenterTypeCode.None:
                case DataCenterTypeCode.Boolean:
                    throw new InvalidCastException();
                case DataCenterTypeCode.Int32:
                    return AsInt32;
                case DataCenterTypeCode.Single:
                    return AsSingle;
                case DataCenterTypeCode.String:
                    return float.TryParse(AsString, NumberStyles.Float, CultureInfo.InvariantCulture,
                        out var f) ? f : fallback ?? throw new InvalidCastException();
                default:
                    throw Assert.Unreachable();
            }
        }

        public double ToDouble(double? fallback = null)
        {
            switch (TypeCode)
            {
                case DataCenterTypeCode.None:
                case DataCenterTypeCode.Boolean:
                    throw new InvalidCastException();
                case DataCenterTypeCode.Int32:
                    return AsInt32;
                case DataCenterTypeCode.Single:
                    return AsSingle;
                case DataCenterTypeCode.String:
                    return double.TryParse(AsString, NumberStyles.Float, CultureInfo.InvariantCulture,
                        out var d) ? d : fallback ?? throw new InvalidCastException();
                default:
                    throw Assert.Unreachable();
            }
        }

        public bool ToBoolean(bool? fallback = null)
        {
            switch (TypeCode)
            {
                case DataCenterTypeCode.None:
                case DataCenterTypeCode.Int32:
                case DataCenterTypeCode.Single:
                    throw new InvalidCastException();
                case DataCenterTypeCode.String:
                    return bool.TryParse(AsString, out var b) ? b :
                        fallback ?? throw new InvalidCastException();
                case DataCenterTypeCode.Boolean:
                    return AsBoolean;
                default:
                    throw Assert.Unreachable();
            }
        }

        public override int GetHashCode()
        {
            var hash = -1521134295;

            hash = hash * -1521134295 + TypeCode.GetHashCode();

            int vhash;

            switch (TypeCode)
            {
                case DataCenterTypeCode.None:
                    vhash = 0;
                    break;
                case DataCenterTypeCode.Int32:
                    vhash = AsInt32.GetHashCode();
                    break;
                case DataCenterTypeCode.Single:
                    vhash = AsSingle.GetHashCode();
                    break;
                case DataCenterTypeCode.String:
                    vhash = AsString.GetHashCode();
                    break;
                case DataCenterTypeCode.Boolean:
                    vhash = AsBoolean.GetHashCode();
                    break;
                default:
                    throw Assert.Unreachable();
            }

            hash = hash * -1521134295 + vhash;

            return hash;
        }

        public override bool Equals(object obj)
        {
            return obj switch
            {
                DataCenterValue v => Equals(v),
                int i => Equals(i),
                float f => Equals(f),
                string s => Equals(s),
                bool b => Equals(b),
                _ => false,
            };
        }

        public override string ToString()
        {
            switch (TypeCode)
            {
                case DataCenterTypeCode.None:
                    return "null";
                case DataCenterTypeCode.Int32:
                    return AsInt32.ToString();
                case DataCenterTypeCode.Single:
                    return AsSingle.ToString();
                case DataCenterTypeCode.String:
                    return AsString;
                case DataCenterTypeCode.Boolean:
                    return AsBoolean.ToString();
                default:
                    throw Assert.Unreachable();
            }
        }

        public static explicit operator int(DataCenterValue value)
        {
            return value.AsInt32;
        }

        public static explicit operator long(DataCenterValue value)
        {
            return value.AsInt64;
        }

        public static explicit operator float(DataCenterValue value)
        {
            return value.AsSingle;
        }

        public static explicit operator double(DataCenterValue value)
        {
            return value.AsDouble;
        }

        public static explicit operator string(DataCenterValue value)
        {
            return value.AsString;
        }

        public static explicit operator bool(DataCenterValue value)
        {
            return value.AsBoolean;
        }

        public static bool operator ==(DataCenterValue left, DataCenterValue right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DataCenterValue left, DataCenterValue right)
        {
            return !left.Equals(right);
        }

        public static bool operator ==(DataCenterValue left, int right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DataCenterValue left, int right)
        {
            return !left.Equals(right);
        }

        public static bool operator ==(DataCenterValue left, float right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DataCenterValue left, float right)
        {
            return !left.Equals(right);
        }

        public static bool operator ==(DataCenterValue left, string right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DataCenterValue left, string right)
        {
            return !left.Equals(right);
        }

        public static bool operator ==(DataCenterValue left, bool right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DataCenterValue left, bool right)
        {
            return !left.Equals(right);
        }
    }
}
