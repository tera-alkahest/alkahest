using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Alkahest.Core.IO
{
    public class TeraBinaryReader : BinaryReader
    {
        public static Encoding Encoding { get; } = Encoding.Unicode;

        public new MemoryStream BaseStream => (MemoryStream)base.BaseStream;

        public int Position
        {
            get { return (int)BaseStream.Position; }
            set { BaseStream.Position = value; }
        }

        public int Length => (int)BaseStream.Length;

        public bool EndOfStream => Position == Length;

        public TeraBinaryReader(byte[] data)
            : base(new MemoryStream(data, false), Encoding)
        {
        }

        public new string ReadString()
        {
            var list = new List<char>();

            while (true)
            {
                var ch = (char)ReadUInt16();

                if (ch == char.MinValue)
                    return new string(list.ToArray());

                list.Add(ch);
            }
        }

        public T Seek<T>(int position, Func<TeraBinaryReader, int, T> func)
        {
            var pos = Position;

            Position = position;

            var ret = func(this, pos);

            Position = pos;

            return ret;
        }

        public void Seek(int position, Action<TeraBinaryReader, int> action)
        {
            Seek(position, (r, op) =>
            {
                action(r, op);
                return (object)null;
            });
        }

        public bool CanRead(int size)
        {
            return Length - Position >= size;
        }
    }
}
