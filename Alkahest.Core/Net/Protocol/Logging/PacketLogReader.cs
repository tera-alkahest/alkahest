using System;
using System.IO;
using System.Linq;

namespace Alkahest.Core.Net.Protocol.Logging
{
    public sealed class PacketLogReader : IDisposable
    {
        public Region Region { get; }

        public DateTime StartTime { get; }

        readonly BinaryReader _reader;

        public PacketLogReader(string fileName)
        {
            _reader = new BinaryReader(File.OpenRead(fileName));

            if (!_reader.ReadBytes(4).SequenceEqual(PacketLogWriter.Magic))
                throw new InvalidDataException();

            Region = (Region)_reader.ReadByte();
            StartTime = DateTime.FromBinary(_reader.ReadInt64());
        }

        public void Dispose()
        {
            _reader.Dispose();
        }

        public PacketLogEntry Read()
        {
            if (_reader.BaseStream.Position == _reader.BaseStream.Length)
                return null;

            var stamp = DateTime.FromBinary(_reader.ReadInt64()).ToLocalTime();
            var name = _reader.ReadString();
            var direction = (Direction)_reader.ReadByte();
            var opCode = _reader.ReadUInt16();
            var length = _reader.ReadUInt16();
            var payload = _reader.ReadBytes(length);

            return new PacketLogEntry(stamp, name, direction, opCode, payload);
        }
    }
}
