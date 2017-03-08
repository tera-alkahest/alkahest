using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Alkahest.Core.Net.Protocol.Logging
{
    public sealed class PacketLogWriter : IDisposable
    {
        public static IReadOnlyList<byte> Magic => _magic;

        static readonly byte[] _magic =
            new[] { 'A', 'T', 'P', 'L' }.Select(x => (byte)x).ToArray();

        public Region Region { get; }

        public DateTime StartTime { get; }

        readonly BinaryWriter _writer;

        public PacketLogWriter(Region region, string directory,
            string fileNameFormat)
        {
            Directory.CreateDirectory(directory);

            _writer = new BinaryWriter(new DeflateStream(new FileStream(
                Path.Combine(directory, DateTime.Now.ToString(fileNameFormat) +
                ".pkt"), FileMode.Create, FileAccess.Write),
                CompressionLevel.Optimal));

            _writer.Write(_magic);
            _writer.Write((byte)(Region = region));
            _writer.Write((StartTime = DateTime.Now).ToUniversalTime().ToBinary());
        }

        public void Dispose()
        {
            _writer.Dispose();
        }

        public void Write(PacketLogEntry entry)
        {
            _writer.Write(entry.Timestamp.ToUniversalTime().ToBinary());
            _writer.Write(entry.ServerName);
            _writer.Write((byte)entry.Direction);
            _writer.Write(entry.OpCode);
            _writer.Write((ushort)entry.Payload.Count);
            _writer.Write(entry.Payload.ToArray());
        }
    }
}
