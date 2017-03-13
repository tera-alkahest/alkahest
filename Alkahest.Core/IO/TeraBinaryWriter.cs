using System;
using System.IO;
using System.Text;
using Alkahest.Core.Net.Protocol;

namespace Alkahest.Core.IO
{
    public class TeraBinaryWriter : BinaryWriter
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

        public TeraBinaryWriter()
            : base(new MemoryStream(PacketHeader.MaxPayloadSize), Encoding)
        {
        }

        public TeraBinaryWriter(byte[] buffer)
            : base(new MemoryStream(buffer), Encoding)
        {
        }

        public new void Write(string value)
        {
            Write(value.ToCharArray());
            Write(char.MinValue);
        }

        public T Seek<T>(int position, Func<TeraBinaryWriter, int, T> func)
        {
            var pos = Position;

            Position = position;

            var ret = func(this, pos);

            Position = pos;

            return ret;
        }

        public void Seek(int position, Action<TeraBinaryWriter, int> action)
        {
            Seek(position, (w, op) =>
            {
                action(w, op);
                return (object)null;
            });
        }

        public bool CanWrite(int size)
        {
            return Length - Position >= size;
        }
    }
}
