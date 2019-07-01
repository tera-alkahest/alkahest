using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Alkahest.Core
{
    public static class Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Memory<T> AsMemory<T>(this ReadOnlyMemory<T> memory)
        {
            return MemoryMarshal.AsMemory(memory);
        }

        public static ArraySegment<T> GetArray<T>(this ReadOnlyMemory<T> memory)
        {
            if (MemoryMarshal.TryGetArray(memory, out var seg))
                return seg;

            throw new InvalidOperationException("Memory does not have a backing array.");
        }

        public static ArraySegment<T> GetArray<T>(this Memory<T> memory)
        {
            return ((ReadOnlyMemory<T>)memory).GetArray();
        }

        public static IEnumerable<T> ToEnumerable<T>(this ReadOnlyMemory<T> memory)
        {
            return MemoryMarshal.ToEnumerable(memory);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<byte> AsBytes<T>(this ReadOnlySpan<T> span)
            where T : struct
        {
            return MemoryMarshal.AsBytes(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<byte> AsBytes<T>(this Span<T> span)
            where T : struct
        {
            return MemoryMarshal.AsBytes(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T GetReference<T>(this ReadOnlySpan<T> span)
        {
            return ref MemoryMarshal.GetReference(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T GetReference<T>(this Span<T> span)
        {
            return ref MemoryMarshal.GetReference(span);
        }

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
