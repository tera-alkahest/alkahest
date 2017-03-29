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

        bool _disposed;

        public PacketLogReader(string fileName)
        {
            Stream stream = File.OpenRead(fileName);

            var magic = new byte[PacketLogEntry.Magic.Count];

            if (stream.Read(magic, 0, magic.Length) != magic.Length ||
                !magic.SequenceEqual(PacketLogEntry.Magic))
                throw new InvalidDataException();

            Region = (Region)stream.ReadByte();

            if (stream.ReadByte() == 1)
                stream = new DeflateStream(stream, CompressionMode.Decompress);

            _reader = new BinaryReader(stream);
        }

        ~PacketLogReader()
        {
            RealDispose();
        }

        public void Dispose()
        {
            RealDispose();
            GC.SuppressFinalize(this);
        }

        void RealDispose()
        {
            _disposed = true;

            _reader.Dispose();
        }

        public PacketLogEntry Read()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

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
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            IEnumerable<PacketLogEntry> Enumerate()
            {
                PacketLogEntry entry;

                while ((entry = Read()) != null)
                    yield return entry;
            }

            return Enumerate();
        }
    }
}
