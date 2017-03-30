using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Sockets;

namespace Alkahest.Core.Net.Protocol.Logging
{
    public sealed class PacketLogWriter : IDisposable
    {
        public Region Region { get; }

        readonly BinaryWriter _writer;

        bool _disposed;

        public PacketLogWriter(Region region, ServerInfo[] servers,
            string directory, string fileNameFormat, bool compress)
        {
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
            _writer.Write(PacketLogEntry.Version);
            _writer.Write((byte)(Region = region));
            _writer.Write(servers.Length);

            foreach (var server in servers)
            {
                _writer.Write(server.Id);
                _writer.Write(server.Name);
                _writer.Write(server.RealAddress.AddressFamily ==
                    AddressFamily.InterNetworkV6);
                _writer.Write(server.RealAddress.GetAddressBytes());
                _writer.Write(server.ProxyAddress.GetAddressBytes());
                _writer.Write((ushort)server.Port);
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
