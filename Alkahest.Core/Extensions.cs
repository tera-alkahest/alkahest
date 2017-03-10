using System;

namespace Alkahest.Core
{
    public static class Extensions
    {
        public static T[] Slice<T>(this T[] array, int start, int length)
        {
            var arr = new T[length];

            Array.Copy(array, start, arr, 0, length);

            return arr;
        }

        public static string ToDirectionString(this Direction direction)
        {
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

        public static string ToRegionString(this Region region)
        {
            switch (region)
            {
                case Region.EU:
                    return "uk";
                case Region.NA:
                    return "en";
                default:
                    throw Assert.Unreachable();
            }
        }
    }
}
