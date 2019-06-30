using Alkahest.Core.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Sockets;

namespace Alkahest.Core.Net.Game.Logging
{
    public sealed class PacketLogWriter : IDisposable
    {
        public bool IsCompressed { get; }

        public uint Version { get; } = PacketLogEntry.Version;

        public Region Region { get; }

        public GameMessageTable GameMessages { get; }

        public SystemMessageTable SystemMessages { get; }

        public IReadOnlyDictionary<int, ServerInfo> Servers { get; }

        readonly GameBinaryWriter _writer;

        bool _disposed;

        public PacketLogWriter(Region region, GameMessageTable gameMessages, SystemMessageTable systemMessages,
            ServerInfo[] servers, string directory, string fileNameFormat, bool compress)
        {
            if (servers == null)
                throw new ArgumentNullException(nameof(servers));

            if (servers.Any(x => x == null))
                throw new ArgumentException("A null server was given.", nameof(servers));

            if (fileNameFormat == null)
                throw new ArgumentNullException(nameof(fileNameFormat));

            IsCompressed = compress;
            Region = region.CheckValidity(nameof(region));
            GameMessages = gameMessages ?? throw new ArgumentNullException(nameof(gameMessages));
            SystemMessages = systemMessages ?? throw new ArgumentNullException(nameof(systemMessages));
            Servers = servers.ToDictionary(x => x.Id);

            Directory.CreateDirectory(directory);

            Stream stream = File.Open(Path.Combine(directory, DateTime.Now.ToString(fileNameFormat) + ".pkt"),
                FileMode.Create, FileAccess.Write);

            using var writer = new GameBinaryWriter(stream, true);

            writer.WriteBytes(PacketLogEntry.Magic.ToArray());
            writer.WriteUInt32(Version);
            writer.WriteByte((byte)Region);
            writer.WriteUInt32(GameMessages.Version);
            writer.WriteUInt32((uint)servers.Length);

            foreach (var server in servers)
            {
                writer.WriteInt32(server.Id);
                writer.WriteString(server.Name);
                writer.WriteBoolean(server.RealEndPoint.AddressFamily == AddressFamily.InterNetworkV6);
                writer.WriteBytes(server.RealEndPoint.Address.GetAddressBytes());
                writer.WriteUInt16((ushort)server.RealEndPoint.Port);
                writer.WriteBytes(server.ProxyEndPoint.Address.GetAddressBytes());
                writer.WriteUInt16((ushort)server.ProxyEndPoint.Port);
            }

            writer.WriteByte((byte)(compress ? 6 : 0));

            _writer = new GameBinaryWriter(compress ?
                new DeflateStream(stream, CompressionLevel.Optimal) : stream);
        }

        ~PacketLogWriter()
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
                _writer.Dispose();
        }

        public void Write(PacketLogEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));

            if (!Servers.ContainsKey(entry.ServerId))
                throw new ArgumentException("Invalid server ID.", nameof(entry));

            if (!GameMessages.CodeToName.ContainsKey(entry.MessageCode))
                throw new ArgumentException("Invalid game message code.", nameof(entry));

            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            _writer.WriteInt64(new DateTimeOffset(entry.Timestamp).ToUnixTimeMilliseconds());
            _writer.WriteInt32(entry.ServerId);
            _writer.WriteByte((byte)entry.Direction);
            _writer.WriteUInt16(entry.MessageCode);
            _writer.WriteUInt16((ushort)entry.Payload.Count);
            _writer.WriteBytes(entry.Payload.ToArray());
        }
    }
}
