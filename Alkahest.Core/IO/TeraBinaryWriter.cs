using System;
using System.IO;
using System.Numerics;
using System.Text;
using Alkahest.Core.Data;
using Alkahest.Core.Net.Protocol;

namespace Alkahest.Core.IO
{
    public sealed class TeraBinaryWriter : IDisposable
    {
        public static Encoding Encoding { get; } = Encoding.Unicode;

        public MemoryStream Stream => (MemoryStream)_writer.BaseStream;

        public int Position
        {
            get => (int)Stream.Position;
            set => Stream.Position = value;
        }

        public int Length => (int)Stream.Length;

        public bool EndOfStream => Position == Length;

        readonly BinaryWriter _writer;

        public TeraBinaryWriter()
        {
            _writer = new BinaryWriter(
                new MemoryStream(PacketHeader.MaxPayloadSize), Encoding);
        }

        public TeraBinaryWriter(byte[] buffer)
        {
            _writer = new BinaryWriter(new MemoryStream(buffer), Encoding);
        }

        public void Dispose()
        {
            _writer.Dispose();
        }

        public void WriteByte(byte value)
        {
            _writer.Write(value);
        }

        public void WriteSByte(sbyte value)
        {
            _writer.Write(value);
        }

        public void WriteUInt16(ushort value)
        {
            _writer.Write(value);
        }

        public void WriteInt16(short value)
        {
            _writer.Write(value);
        }

        public void WriteUInt32(uint value)
        {
            _writer.Write(value);
        }

        public void WriteInt32(int value)
        {
            _writer.Write(value);
        }

        public void WriteUInt64(ulong value)
        {
            _writer.Write(value);
        }

        public void WriteInt64(long value)
        {
            _writer.Write(value);
        }

        public void WriteSingle(float value)
        {
            _writer.Write(value);
        }

        public void WriteBoolean(bool value)
        {
            _writer.Write(value);
        }

        public void WriteString(string value)
        {
            _writer.Write(value.ToCharArray());
            _writer.Write(char.MinValue);
        }

        public void WriteVector3(Vector3 value)
        {
            _writer.Write(value.X);
            _writer.Write(value.Y);
            _writer.Write(value.Z);
        }

        public void WriteEntityId(EntityId value)
        {
            _writer.Write(value.Raw);
        }

        public void WriteSkillId(SkillId value)
        {
            _writer.Write(value.Raw);
        }

        public void WriteAngle(Angle value)
        {
            _writer.Write(value.Raw);
        }

        public void WriteTemplateId(TemplateId value)
        {
            _writer.Write(value.Raw);
        }

        public void WriteOffset(int value)
        {
            _writer.Write((ushort)(value + PacketHeader.HeaderSize));
        }

        public void WriteBytes(byte[] value)
        {
            _writer.Write(value);
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
