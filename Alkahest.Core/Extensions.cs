using System;

namespace Alkahest.Core
{
    public static class Extensions
    {
        internal static void CheckValidity(this Enum value, string name)
        {
            var t = value.GetType();

            if (!Enum.IsDefined(t, value))
                throw new ArgumentException($"Invalid {t.Name} value.", name);
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
            direction.CheckValidity(nameof(direction));

            switch (direction)
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
