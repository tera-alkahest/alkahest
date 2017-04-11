using System.IO;

namespace Alkahest.Core.IO
{
    public static class IOExtensions
    {
        public static byte[] ReadBytesFull(this BinaryReader reader, int count)
        {
            var bytes = reader.ReadBytes(count);

            if (bytes.Length != count)
                throw new EndOfStreamException();

            return bytes;
        }
    }
}
