using System;
using System.Globalization;

namespace Alkahest.Core.Data
{
    public struct DataCenterAttribute
    {
        const NumberStyles Int32Styles =
            NumberStyles.Integer |
            NumberStyles.AllowHexSpecifier;

        const NumberStyles SingleStyles = NumberStyles.Float;

        public readonly string Name;

        public readonly DataCenterTypeCode TypeCode;

        public object Value
        {
            get
            {
                switch (TypeCode)
                {
                    case DataCenterTypeCode.None:
                        throw new InvalidOperationException();
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

        public unsafe float AsSingle
        {
            get
            {
                if (TypeCode != DataCenterTypeCode.Single)
                    throw new InvalidOperationException();

                var value = _primitiveValue;

                return *(float*)&value;
            }
        }

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

        public bool HasValue => TypeCode != DataCenterTypeCode.None;

        readonly int _primitiveValue;

        readonly string _stringValue;

        internal DataCenterAttribute(string name, DataCenterTypeCode typeCode,
            int primitiveValue, string stringValue)
        {
            Name = name;
            TypeCode = typeCode;
            _primitiveValue = primitiveValue;
            _stringValue = stringValue;
        }

        public int ToInt32()
        {
            switch (TypeCode)
            {
                case DataCenterTypeCode.Int32:
                    return AsInt32;
                case DataCenterTypeCode.String:
                    return int.Parse(AsString, Int32Styles, CultureInfo.InvariantCulture);
                case DataCenterTypeCode.None:
                case DataCenterTypeCode.Single:
                case DataCenterTypeCode.Boolean:
                    throw new InvalidOperationException();
                default:
                    throw Assert.Unreachable();
            }
        }

        public float ToSingle()
        {
            switch (TypeCode)
            {
                case DataCenterTypeCode.Single:
                    return AsSingle;
                case DataCenterTypeCode.String:
                    return float.Parse(AsString, SingleStyles, CultureInfo.InvariantCulture);
                case DataCenterTypeCode.None:
                case DataCenterTypeCode.Int32:
                case DataCenterTypeCode.Boolean:
                    throw new InvalidOperationException();
                default:
                    throw Assert.Unreachable();
            }
        }

        public bool ToBoolean()
        {
            switch (TypeCode)
            {
                case DataCenterTypeCode.Boolean:
                    return AsBoolean;
                case DataCenterTypeCode.String:
                    return bool.Parse(AsString);
                case DataCenterTypeCode.None:
                case DataCenterTypeCode.Int32:
                case DataCenterTypeCode.Single:
                    throw new InvalidOperationException();
                default:
                    throw Assert.Unreachable();
            }
        }

        public override string ToString()
        {
            string value;

            switch (TypeCode)
            {
                case DataCenterTypeCode.None:
                    value = "N/A";
                    break;
                case DataCenterTypeCode.Int32:
                    value = AsInt32.ToString();
                    break;
                case DataCenterTypeCode.Single:
                    value = AsSingle.ToString();
                    break;
                case DataCenterTypeCode.String:
                    value = $"\"{AsString}\"";
                    break;
                case DataCenterTypeCode.Boolean:
                    value = AsBoolean.ToString();
                    break;
                default:
                    throw Assert.Unreachable();
            }

            return $"[Name: {Name}, Type: {TypeCode}, Value: {value}]";
        }

        public static explicit operator int(DataCenterAttribute attribute)
        {
            return attribute.AsInt32;
        }

        public static explicit operator float(DataCenterAttribute attribute)
        {
            return attribute.AsSingle;
        }

        public static explicit operator bool(DataCenterAttribute attribute)
        {
            return attribute.AsBoolean;
        }

        public static explicit operator string(DataCenterAttribute attribute)
        {
            return attribute.AsString;
        }
    }
}
