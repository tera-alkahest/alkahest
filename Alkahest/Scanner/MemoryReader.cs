using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Alkahest.Scanner
{
    sealed class MemoryReader
    {
        public IntPtr Address { get; }

        public int Length { get; }

        public MemoryReader(IntPtr baseAddress, int length)
        {
            Address = baseAddress;
            Length = length;
        }

        public IntPtr ToAbsolute(int offset)
        {
            return Address + offset;
        }

        public int ToRelative(IntPtr address)
        {
            return address.ToInt32() - Address.ToInt32();
        }

        public bool IsInRange(int offset)
        {
            return offset >= 0 && offset < Length;
        }

        public IEnumerable<int> FindOffset(params byte?[][] patterns)
        {
            bool IsMatch(byte?[] pattern, int offset)
            {
                for (var j = 0; j < pattern.Length; j++)
                {
                    var b = pattern[j];

                    if (b != null && Read<byte>(offset + j) != b)
                        return false;
                }

                return true;
            }

            return (from i in Enumerable.Range(0, Length).AsParallel()
                    from p in patterns
                    where i + p.Length < Length
                    where IsMatch(p, i)
                    select i).Distinct();
        }

        public unsafe T Read<T>(int offset)
            where T : unmanaged
        {
            return Unsafe.Read<T>(ToAbsolute(offset).ToPointer());
        }

        public int ReadOffset(int offset)
        {
            return ToRelative(Read<IntPtr>(offset));
        }

        public byte[] ReadBytes(int offset, int count)
        {
            var bytes = new byte[count];

            Marshal.Copy(ToAbsolute(offset), bytes, 0, count);

            return bytes;
        }

        public T ReadStructure<T>(int offset)
            where T : struct
        {
            return Marshal.PtrToStructure<T>(ToAbsolute(offset));
        }

        public T GetDelegate<T>(int offset)
            where T : Delegate
        {
            return Marshal.GetDelegateForFunctionPointer<T>(ToAbsolute(offset));
        }
    }
}
