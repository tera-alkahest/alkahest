using Alkahest.Core.IO;
using Alkahest.Core.Net.Protocol.OpCodes;
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
        public bool IsCompressed { get; }

        public uint Version { get; }

        public MessageTables Messages { get; }

        public IReadOnlyDictionary<int, ServerInfo> Servers { get; }

        readonly TeraBinaryReader _reader;

        bool _disposed;

        public PacketLogReader(string fileName)
        {
            Stream stream = File.OpenRead(fileName);

            var magic = new byte[PacketLogEntry.Magic.Count];

            if (stream.Read(magic, 0, magic.Length) != magic.Length)
                throw new EndOfStreamException();

            if (!magic.SequenceEqual(PacketLogEntry.Magic))
                throw new InvalidDataException();

            IsCompressed = stream.ReadByte() != 0;

            if (IsCompressed)
                stream = new DeflateStream(stream, CompressionMode.Decompress);

            _reader = new TeraBinaryReader(stream);
            Version = _reader.ReadUInt32();

            if (Version != PacketLogEntry.Version)
                throw new InvalidDataException();

            var region = (Region)_reader.ReadByte();

            if (!Enum.IsDefined(typeof(Region), region))
                throw new InvalidDataException();

            var clientVersion = _reader.ReadUInt32();

            if (!OpCodeTable.Versions.Values.Contains(clientVersion))
                throw new InvalidDataException();

            Messages = new MessageTables(region, clientVersion);

            var serverCount = (int)_reader.ReadUInt32();

            if (serverCount < 0)
                throw new InvalidDataException();

            var servers = new Dictionary<int, ServerInfo>(serverCount);

            for (var i = 0; i < serverCount; i++)
            {
                var id = _reader.ReadInt32();

                if (servers.ContainsKey(id))
                    throw new InvalidDataException();

                var name = _reader.ReadString();
                var size = _reader.ReadBoolean() ? 16 : 4;
                var realIPBytes = _reader.ReadBytes(size);
                var realPort = _reader.ReadUInt16();
                var proxyIPBytes = _reader.ReadBytes(size);
                var proxyPort = _reader.ReadUInt16();

                IPEndPoint realEP;
                IPEndPoint proxyEP;

                try
                {
                    realEP = new IPEndPoint(new IPAddress(realIPBytes), realPort);
                    proxyEP = new IPEndPoint(new IPAddress(proxyIPBytes), proxyPort);
                }
                catch (ArgumentException)
                {
                    throw new InvalidDataException();
                }

                servers.Add(id, new ServerInfo(id, name, realEP, proxyEP));
            }

            Servers = servers;
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
                DateTime stamp;

                try
                {
                    stamp = DateTimeOffset.FromUnixTimeMilliseconds(
                        _reader.ReadInt64()).LocalDateTime;
                }
                catch (ArgumentOutOfRangeException)
                {
                    throw new InvalidDataException();
                }

                var id = _reader.ReadInt32();

                if (!Servers.ContainsKey(id))
                    throw new InvalidDataException();

                var direction = (Direction)_reader.ReadByte();

                if (!Enum.IsDefined(typeof(Direction), direction))
                    throw new InvalidDataException();

                var opCode = _reader.ReadUInt16();

                if (!Messages.Game.OpCodeToName.ContainsKey(opCode))
                    throw new InvalidDataException();

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
