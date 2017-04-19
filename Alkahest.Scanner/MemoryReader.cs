using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Alkahest.Scanner
{
    sealed class MemoryReader
    {
        public IntPtr BaseAddress { get; }

        public int Length { get; }

        public MemoryReader(IntPtr baseAddress, int length)
        {
            BaseAddress = baseAddress;
            Length = length;
        }

        public IntPtr ToAbsolute(int offset)
        {
            return BaseAddress + offset;
        }

        public int ToOffset(IntPtr address)
        {
            return address.ToInt32() - BaseAddress.ToInt32();
        }

        public bool IsValid(int offset)
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

                    if (b != null && ReadByte(offset + j) != b)
                        return false;
                }

                return true;
            }

            return (from i in Enumerable.Range(0, Length)
                    from p in patterns
                    where i + p.Length < Length
                    where IsMatch(p, i)
                    select i).Distinct();
        }

        public byte ReadByte(int offset)
        {
            return Marshal.ReadByte(ToAbsolute(offset));
        }

        public sbyte ReadSByte(int offset)
        {
            return (sbyte)Marshal.ReadByte(ToAbsolute(offset));
        }

        public short ReadInt16(int offset)
        {
            return Marshal.ReadInt16(ToAbsolute(offset));
        }

        public ushort ReadUInt16(int offset)
        {
            return (ushort)Marshal.ReadInt16(ToAbsolute(offset));
        }

        public int ReadInt32(int offset)
        {
            return Marshal.ReadInt32(ToAbsolute(offset));
        }

        public uint ReadUInt32(int offset)
        {
            return (uint)Marshal.ReadInt32(ToAbsolute(offset));
        }

        public long ReadInt64(int offset)
        {
            return Marshal.ReadInt64(ToAbsolute(offset));
        }

        public ulong ReadUInt64(int offset)
        {
            return (ulong)Marshal.ReadInt64(ToAbsolute(offset));
        }

        public unsafe float ReadSingle(int offset)
        {
            var value = ReadInt32(offset);

            return *(float*)&value;
        }

        public unsafe double ReadDouble(int offset)
        {
            var value = ReadInt64(offset);

            return *(double*)&value;
        }

        public int ReadOffset(int offset)
        {
            return ToOffset(Marshal.ReadIntPtr(ToAbsolute(offset)));
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

        public TDelegate GetDelegate<TDelegate>(int offset)
            where TDelegate : class
        {
            return Marshal.GetDelegateForFunctionPointer<TDelegate>(
                ToAbsolute(offset));
        }
    }
}
