using System.Linq;

namespace Alkahest.Core
{
    public static class Bits
    {
        public static int Insert(int value1, int value2, int start, int count)
        {
            var mask = (1 << count) - 1;

            return value1 & ~(mask << start) | (value2 & mask) << start;
        }

        public static uint Insert(uint value1, uint value2, int start, int count)
        {
            var mask = (1U << count) - 1;

            return value1 & ~(mask << start) | (value2 & mask) << start;
        }

        public static long Insert(long value1, long value2, int start, int count)
        {
            var mask = (1L << count) - 1;

            return value1 & ~(mask << start) | (value2 & mask) << start;
        }

        public static ulong Insert(ulong value1, ulong value2, int start, int count)
        {
            var mask = (1UL << count) - 1;

            return value1 & ~(mask << start) | (value2 & mask) << start;
        }

        public static int Extract(int value, int start, int count)
        {
            return value >> start & (1 << count) - 1;
        }

        public static uint Extract(uint value, int start, int count)
        {
            return value >> start & (1U << count) - 1;
        }

        public static long Extract(long value, int start, int count)
        {
            return value >> start & (1L << count) - 1;
        }

        public static ulong Extract(ulong value, int start, int count)
        {
            return value >> start & (1UL << count) - 1;
        }

        public static int Clear(int value, int bit)
        {
            return value & ~(1 << bit);
        }

        public static uint Clear(uint value, int bit)
        {
            return value & ~(1U << bit);
        }

        public static long Clear(long value, int bit)
        {
            return value & ~(1L << bit);
        }

        public static ulong Clear(ulong value, int bit)
        {
            return value & ~(1UL << bit);
        }

        public static int Set(int value, int bit)
        {
            return value | (1 << bit);
        }

        public static uint Set(uint value, int bit)
        {
            return value | (1U << bit);
        }

        public static long Set(long value, int bit)
        {
            return value | (1L << bit);
        }

        public static ulong Set(ulong value, int bit)
        {
            return value | (1UL << bit);
        }

        public static int Toggle(int value, int bit)
        {
            return value ^ 1 << bit;
        }

        public static uint Toggle(uint value, int bit)
        {
            return value ^ 1U << bit;
        }

        public static long Toggle(long value, int bit)
        {
            return value ^ 1L << bit;
        }

        public static ulong Toggle(ulong value, int bit)
        {
            return value ^ 1UL << bit;
        }

        public static bool Check(int value, int bit)
        {
            return (value & 1 << bit) != 0;
        }

        public static bool Check(uint value, int bit)
        {
            return (value & 1U << bit) != 0;
        }

        public static bool Check(long value, int bit)
        {
            return (value & 1L << bit) != 0;
        }

        public static bool Check(ulong value, int bit)
        {
            return (value & 1UL << bit) != 0;
        }

        public static int Compose(params (int value, int start, int count)[] values)
        {
            var result = 0;

            // Avoid LINQ for performance reasons.
            foreach (var (value, start, count) in values)
                result = Insert(result, value, start, count);

            return result;
        }

        public static uint Compose(params (uint value, int start, int count)[] values)
        {
            var result = 0U;

            // Avoid LINQ for performance reasons.
            foreach (var (value, start, count) in values)
                result = Insert(result, value, start, count);

            return result;
        }

        public static long Compose(params (long value, int start, int count)[] values)
        {
            var result = 0L;

            // Avoid LINQ for performance reasons.
            foreach (var (value, start, count) in values)
                result = Insert(result, value, start, count);

            return result;
        }

        public static ulong Compose(params (ulong value, int start, int count)[] values)
        {
            var result = 0UL;

            // Avoid LINQ for performance reasons.
            foreach (var (value, start, count) in values)
                result = Insert(result, value, start, count);

            return result;
        }
    }
}
