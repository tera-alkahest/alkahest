using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Sockets;
using Alkahest.Core.Net.Protocol.OpCodes;

namespace Alkahest.Core.Net.Protocol.Logging
{
    public sealed class PacketLogWriter : IDisposable
    {
        public bool Compressed { get; }

        public int Version { get; }

        public Region Region { get; }

        public MessageTables Messages { get; }

        public IReadOnlyDictionary<int, ServerInfo> Servers { get; }

        readonly BinaryWriter _writer;

        bool _disposed;

        public PacketLogWriter(Region region, MessageTables messages,
            ServerInfo[] servers, string directory, string fileNameFormat,
            bool compress)
        {
            region.CheckValidity(nameof(region));

            if (servers == null)
                throw new ArgumentNullException(nameof(servers));

            if (servers.Any(x => x == null))
                throw new ArgumentException("A null server was given.",
                    nameof(servers));

            if (fileNameFormat == null)
                throw new ArgumentNullException(nameof(fileNameFormat));

            Compressed = compress;
            Version = PacketLogEntry.Version;
            Region = region;
            Messages = messages ??
                throw new ArgumentNullException(nameof(messages));
            Servers = servers.ToDictionary(x => x.Id);

            Directory.CreateDirectory(directory);

            Stream stream = File.Open(Path.Combine(directory,
                DateTime.Now.ToString(fileNameFormat) + ".pkt"),
                FileMode.Create, FileAccess.Write);

            var magic = PacketLogEntry.Magic.ToArray();

            stream.Write(magic, 0, magic.Length);
            stream.WriteByte((byte)(compress ? 1 : 0));

            if (compress)
                stream = new DeflateStream(stream, CompressionLevel.Optimal);

            _writer = new BinaryWriter(stream);
            _writer.Write(Version);
            _writer.Write((byte)region);
            _writer.Write(messages.Game.Version);
            _writer.Write(servers.Length);

            foreach (var server in servers)
            {
                _writer.Write(server.Id);
                _writer.Write(server.Name);
                _writer.Write(server.RealEndPoint.AddressFamily ==
                    AddressFamily.InterNetworkV6);
                _writer.Write(server.RealEndPoint.Address.GetAddressBytes());
                _writer.Write((ushort)server.RealEndPoint.Port);
                _writer.Write(server.ProxyEndPoint.Address.GetAddressBytes());
                _writer.Write((ushort)server.ProxyEndPoint.Port);
            }
        }

        ~PacketLogWriter()
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

            _writer?.Dispose();
        }

        public void Write(PacketLogEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));

            if (!Servers.ContainsKey(entry.ServerId))
                throw new ArgumentException("Invalid server ID.", nameof(entry));

            if (!Messages.Game.OpCodeToName.ContainsKey(entry.OpCode))
                throw new ArgumentException("Invalid opcode.", nameof(entry));

            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            _writer.Write(entry.Timestamp.ToUniversalTime().ToBinary());
            _writer.Write(entry.ServerId);
            _writer.Write((byte)entry.Direction);
            _writer.Write(entry.OpCode);
            _writer.Write((ushort)entry.Payload.Count);
            _writer.Write(entry.Payload.ToArray());
        }
    }
}
