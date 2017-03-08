using System;
using System.IO;
using System.Text;
using Alkahest.Core.Net.Protocol;

namespace Alkahest.Core.IO
{
    public class TeraBinaryWriter : BinaryWriter
    {
        public static Encoding Encoding => Encoding.Unicode;

        public new MemoryStream BaseStream => (MemoryStream)base.BaseStream;

        public int Position
        {
            get { return (int)BaseStream.Position; }
            set { BaseStream.Position = value; }
        }

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
            foreach (var ch in value)
                Write(ch);

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
    }
}
