using System.IO;
using System.IO.Compression;

namespace Alkahest.Core.IO
{
    public class FastDeflateStream : DeflateStream
    {
        readonly byte[] _byte = new byte[1];

        public FastDeflateStream(Stream stream, CompressionMode mode, bool leaveOpen = false)
            : base(stream, mode, leaveOpen)
        {
        }

        public FastDeflateStream(Stream stream, CompressionLevel compressionLevel, bool leaveOpen = false)
            : base(stream, compressionLevel, leaveOpen)
        {
        }

        public override int ReadByte()
        {
            return Read(_byte, 0, sizeof(byte)) == 0 ? -1 : _byte[0];
        }

        public override void WriteByte(byte value)
        {
            _byte[0] = value;

            Write(_byte, 0, sizeof(byte));
        }
    }
}
