using System;

namespace Alkahest.Core
{
    public static class Extensions
    {
        internal static T CheckValidity<T>(this T value, string name)
            where T : Enum
        {
            var t = typeof(T);

            if (!Enum.IsDefined(t, value))
                throw new ArgumentException($"Invalid {t.Name} value.", name);

            return value;
        }

        internal static T CheckFlagsValidity<T>(this T value, string name)
            where T : Enum
        {
            var t = typeof(T);
            var ch = value.ToString()[0];

            if (ch == '-' || char.IsDigit(ch))
                throw new ArgumentException($"Invalid {t.Name} flag combination.", name);

            return value;
        }

        public static T[] Slice<T>(this T[] array, int start, int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            var arr = new T[length];

            Array.Copy(array, start, arr, 0, length);

            return arr;
        }

        public static string ToDirectionString(this Direction direction)
        {
            switch (direction.CheckValidity(nameof(direction)))
            {
                case Direction.ClientToServer:
                    return "C -> S";
                case Direction.ServerToClient:
                    return "S -> C";
                default:
                    throw Assert.Unreachable();
            }
        }
    }
}
