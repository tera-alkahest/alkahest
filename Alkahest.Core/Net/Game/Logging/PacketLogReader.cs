using Alkahest.Core.Data;
using Alkahest.Core.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;

namespace Alkahest.Core.Net.Game.Logging
{
    public sealed class PacketLogReader : IDisposable
    {
        public byte CompressionLevel { get; }

        public uint Version { get; }

        public Region Region { get; }

        public GameMessageTable GameMessages { get; }

        public SystemMessageTable SystemMessages { get; }

        public IReadOnlyDictionary<int, ServerInfo> Servers { get; }

        readonly GameBinaryReader _reader;

        bool _disposed;

        public PacketLogReader(string fileName)
        {
            Stream stream = File.OpenRead(fileName);

            using var reader = new GameBinaryReader(stream, true);

            if (!reader.ReadBytes(PacketLogEntry.Magic.Count).SequenceEqual(PacketLogEntry.Magic))
                throw new InvalidDataException("Invalid magic number.");

            Version = reader.ReadUInt32();

            if (Version != PacketLogEntry.Version)
                throw new InvalidDataException($"Unknown format version {Version}.");

            Region = (Region)reader.ReadByte();

            if (!Enum.IsDefined(typeof(Region), Region))
                throw new InvalidDataException($"Unknown region value {Region}.");

            var clientVersion = reader.ReadUInt32();

            if (!DataCenter.ClientVersions.Values.Contains(clientVersion))
                throw new InvalidDataException($"Unknown client version {clientVersion}.");

            GameMessages = new GameMessageTable(clientVersion);
            SystemMessages = new SystemMessageTable(clientVersion);

            var serverCount = (int)reader.ReadUInt32();
            var servers = new Dictionary<int, ServerInfo>(serverCount);

            for (var i = 0; i < serverCount; i++)
            {
                var id = reader.ReadInt32();

                if (servers.ContainsKey(id))
                    throw new InvalidDataException($"Duplicate server ID {id}.");

                var name = reader.ReadString();
                var size = reader.ReadBoolean() ? 16 : 4;
                var realIPBytes = reader.ReadBytes(size);
                var realPort = reader.ReadUInt16();
                var proxyIPBytes = reader.ReadBytes(size);
                var proxyPort = reader.ReadUInt16();

                IPEndPoint realEP;

                try
                {
                    realEP = new IPEndPoint(new IPAddress(realIPBytes), realPort);
                }
                catch (ArgumentOutOfRangeException)
                {
                    throw new InvalidDataException($"Invalid real port {realPort}.");
                }
                catch (ArgumentException)
                {
                    throw new InvalidDataException("Invalid real IP address.");
                }

                IPEndPoint proxyEP;

                try
                {
                    proxyEP = new IPEndPoint(new IPAddress(proxyIPBytes), proxyPort);
                }
                catch (ArgumentOutOfRangeException)
                {
                    throw new InvalidDataException($"Invalid proxy port {proxyPort}.");
                }
                catch (ArgumentException)
                {
                    throw new InvalidDataException("Invalid proxy IP address.");
                }

                servers.Add(id, new ServerInfo(id, name, realEP, proxyEP));
            }

            Servers = servers;
            CompressionLevel = reader.ReadByte();

            _reader = new GameBinaryReader(CompressionLevel != 0 ?
                new FastDeflateStream(stream, CompressionMode.Decompress) : stream);
        }

        ~PacketLogReader()
        {
            RealDispose(false);
        }

        public void Dispose()
        {
            RealDispose(true);
            GC.SuppressFinalize(this);
        }

        void RealDispose(bool disposing)
        {
            _disposed = true;

            if (disposing)
                _reader.Dispose();
        }

        public PacketLogEntry Read()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            try
            {
                var time = _reader.ReadInt64();

                DateTime stamp;

                try
                {
                    stamp = DateTimeOffset.FromUnixTimeMilliseconds(time).LocalDateTime;
                }
                catch (ArgumentOutOfRangeException)
                {
                    throw new InvalidDataException($"Invalid timestamp value {time}.");
                }

                var id = _reader.ReadInt32();

                if (!Servers.ContainsKey(id))
                    throw new InvalidDataException($"Invalid server ID {id}.");

                var direction = (Direction)_reader.ReadByte();

                if (!Enum.IsDefined(typeof(Direction), direction))
                    throw new InvalidDataException($"Unknown direction value {direction}.");

                var code = _reader.ReadUInt16();

                if (!GameMessages.CodeToName.ContainsKey(code))
                    throw new InvalidDataException($"Unknown message code {code}.");

                var length = _reader.ReadUInt16();
                var payload = _reader.ReadBytes(length);

                return new PacketLogEntry(stamp, id, direction, code, payload);
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
