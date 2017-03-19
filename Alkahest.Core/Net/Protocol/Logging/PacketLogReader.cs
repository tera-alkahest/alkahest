using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Alkahest.Core.Net.Protocol.Logging
{
    public sealed class PacketLogReader : IDisposable
    {
        public Region Region { get; }

        readonly BinaryReader _reader;

        public PacketLogReader(string fileName)
        {
            Stream stream = File.OpenRead(fileName);

            var magic = new byte[PacketLogWriter.Magic.Count];

            if (stream.Read(magic, 0, magic.Length) != magic.Length ||
                !magic.SequenceEqual(PacketLogWriter.Magic))
                throw new InvalidDataException();

            Region = (Region)stream.ReadByte();

            if (stream.ReadByte() == 1)
                stream = new DeflateStream(stream, CompressionMode.Decompress);

            _reader = new BinaryReader(stream);
        }

        public void Dispose()
        {
            _reader.Dispose();
        }

        public PacketLogEntry Read()
        {
            try
            {
                var stamp = DateTime.FromBinary(_reader.ReadInt64()).ToLocalTime();
                var name = _reader.ReadString();
                var direction = (Direction)_reader.ReadByte();
                var opCode = _reader.ReadUInt16();
                var length = _reader.ReadUInt16();
                var payload = _reader.ReadBytes(length);

                return new PacketLogEntry(stamp, name, direction, opCode, payload);
            }
            catch (EndOfStreamException)
            {
                return null;
            }
        }

        public PacketLogEntry[] ReadAll()
        {
            return EnumerateAll().ToArray();
        }

        public IEnumerable<PacketLogEntry> EnumerateAll()
        {
            PacketLogEntry entry;

            while ((entry = Read()) != null)
                yield return entry;
        }
    }
}
