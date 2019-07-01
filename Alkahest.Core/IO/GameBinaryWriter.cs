using Alkahest.Core.Game;
using Alkahest.Core.Net.Game;
using System;
using System.IO;
using System.Numerics;
using System.Text;

namespace Alkahest.Core.IO
{
    public sealed class GameBinaryWriter : IDisposable
    {
        public static Encoding Encoding { get; } = Encoding.Unicode;

        public Stream Stream => _writer.BaseStream;

        public int Position
        {
            get => (int)Stream.Position;
            set => Stream.Position = value;
        }

        public int Length => (int)Stream.Length;

        public bool EndOfStream => Position >= Length;

        readonly BinaryWriter _writer;

        public GameBinaryWriter()
            : this(new MemoryStream(PacketHeader.MaxPayloadSize))
        {
        }

        public GameBinaryWriter(byte[] buffer)
            : this(new MemoryStream(buffer))
        {
        }

        public GameBinaryWriter(byte[] buffer, int index, int count)
            : this(new MemoryStream(buffer, index, count))
        {
        }

        public GameBinaryWriter(ArraySegment<byte> segment)
            : this(segment.Array, segment.Offset, segment.Count)
        {
        }

        public GameBinaryWriter(Stream stream, bool leaveOpen = false)
        {
            _writer = new BinaryWriter(stream, Encoding, leaveOpen);
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

        public void WriteDouble(double value)
        {
            _writer.Write(value);
        }

        public void WriteBoolean(bool value)
        {
            _writer.Write(value);
        }

        public void WriteString(string value)
        {
            if (!string.IsNullOrEmpty(value))
                _writer.Write(Encoding.GetBytes(value));

            _writer.Write((ushort)char.MinValue);
        }

        public void WriteVector3(Vector3 value)
        {
            _writer.Write(value.X);
            _writer.Write(value.Y);
            _writer.Write(value.Z);
        }

        public void WriteGameId(GameId value)
        {
            _writer.Write(value.Raw);
        }

        public void WriteSkillId(SkillId value)
        {
            _writer.Write(value.Raw);
        }

        public void WriteSimpleSkillId(SkillId value)
        {
            _writer.Write(value.Id);
        }

        public void WriteAngle(Angle value)
        {
            _writer.Write(value.Raw);
        }

        public void WriteTemplateId(TemplateId value)
        {
            _writer.Write(value.Raw);
        }

        public void WriteAppearance(Appearance value)
        {
            _writer.Write(value.Raw);
        }

        public void WriteOffset(int value)
        {
            _writer.Write((ushort)(value + PacketHeader.HeaderSize));
        }

        public void WriteBytes(ReadOnlySpan<byte> value)
        {
            foreach (var b in value)
                _writer.Write(b);
        }

        public T Seek<T>(int position, Func<GameBinaryWriter, int, T> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            var pos = Position;

            Position = position;

            // This method is intentionally not using try/finally.
            var ret = func(this, pos);

            Position = pos;

            return ret;
        }

        public void Seek(int position, Action<GameBinaryWriter, int> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            Seek(position, (w, op) =>
            {
                action(w, op);

                return (object)null;
            });
        }

        public bool CanWrite(int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            return Length - Position >= count;
        }

        public byte[] ToArray()
        {
            using var stream = new MemoryStream(PacketHeader.MaxPayloadSize);

            var pos = Position;

            try
            {
                Position = 0;

                Stream.CopyTo(stream);

                return stream.ToArray();
            }
            finally
            {
                Position = pos;
            }
        }
    }
}
