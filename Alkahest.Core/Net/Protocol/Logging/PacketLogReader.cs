using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;

namespace Alkahest.Core.Net.Protocol.Logging
{
    public sealed class PacketLogReader : IDisposable
    {
        public bool Compressed { get; }

        public int Version { get; }

        public Region Region { get; }

        public IReadOnlyDictionary<int, ServerInfo> Servers { get; }

        readonly BinaryReader _reader;

        bool _disposed;

        public PacketLogReader(string fileName)
        {
            Stream stream = File.OpenRead(fileName);

            var magic = new byte[PacketLogEntry.Magic.Count];

            if (stream.Read(magic, 0, magic.Length) != magic.Length)
                throw new EndOfStreamException();

            if (!magic.SequenceEqual(PacketLogEntry.Magic))
                throw new InvalidDataException();

            Compressed = stream.ReadByte() != 0;

            if (Compressed)
                stream = new DeflateStream(stream, CompressionMode.Decompress);

            _reader = new BinaryReader(stream);
            Version = _reader.ReadInt32();

            if (Version != PacketLogEntry.Version)
                throw new InvalidDataException();

            Region = (Region)_reader.ReadByte();

            var servers = new List<ServerInfo>(_reader.ReadInt32());

            for (var i = 0; i < servers.Capacity; i++)
            {
                var id = _reader.ReadInt32();
                var name = _reader.ReadString();
                var size = _reader.ReadBoolean() ? 16 : 4;
                var realIP = new IPAddress(_reader.ReadBytes(size));
                var realPort = _reader.ReadUInt16();
                var proxyIP = new IPAddress(_reader.ReadBytes(size));
                var proxyPort = _reader.ReadUInt16();

                servers.Add(new ServerInfo(id, name, new IPEndPoint(realIP,
                    realPort), new IPEndPoint(proxyIP, proxyPort)));
            }

            Servers = servers.ToDictionary(x => x.Id);
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

            _reader?.Dispose();
        }

        public PacketLogEntry Read()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            try
            {
                var stamp = DateTime.FromBinary(_reader.ReadInt64()).ToLocalTime();
                var id = _reader.ReadInt32();
                var direction = (Direction)_reader.ReadByte();
                var opCode = _reader.ReadUInt16();
                var length = _reader.ReadUInt16();
                var payload = _reader.ReadBytes(length);

                return new PacketLogEntry(stamp, id, direction, opCode, payload);
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
