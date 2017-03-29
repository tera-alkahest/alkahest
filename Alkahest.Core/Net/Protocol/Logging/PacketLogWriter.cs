using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Alkahest.Core.Net.Protocol.Logging
{
    public sealed class PacketLogWriter : IDisposable
    {
        public Region Region { get; }

        readonly BinaryWriter _writer;

        bool _disposed;

        public PacketLogWriter(Region region, string directory,
            string fileNameFormat, bool compress)
        {
            Directory.CreateDirectory(directory);

            Stream stream = new FileStream(Path.Combine(directory,
                DateTime.Now.ToString(fileNameFormat) + ".pkt"),
                FileMode.Create, FileAccess.Write);

            var magic = PacketLogEntry.Magic.ToArray();

            stream.Write(magic, 0, magic.Length);
            stream.WriteByte((byte)(Region = region));
            stream.WriteByte((byte)(compress ? 1 : 0));

            if (compress)
                stream = new DeflateStream(stream, CompressionLevel.Optimal);

            _writer = new BinaryWriter(stream);
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

            _writer.Dispose();
        }

        public void Write(PacketLogEntry entry)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            _writer.Write(entry.Timestamp.ToUniversalTime().ToBinary());
            _writer.Write(entry.ServerName);
            _writer.Write((byte)entry.Direction);
            _writer.Write(entry.OpCode);
            _writer.Write((ushort)entry.Payload.Count);
            _writer.Write(entry.Payload.ToArray());

            _writer.Flush();
        }
    }
}
