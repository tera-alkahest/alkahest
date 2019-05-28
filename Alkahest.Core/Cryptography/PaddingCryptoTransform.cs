using System;
using System.Security.Cryptography;

namespace Alkahest.Core.Cryptography
{
    public sealed class PaddingCryptoTransform : ICryptoTransform
    {
        public int InputBlockSize => Transform.InputBlockSize;

        public int OutputBlockSize => Transform.OutputBlockSize;

        public bool CanTransformMultipleBlocks => Transform.CanTransformMultipleBlocks;

        public bool CanReuseTransform => Transform.CanReuseTransform;

        public ICryptoTransform Transform { get; }

        bool _disposed;

        public PaddingCryptoTransform(ICryptoTransform transform)
        {
            Transform = transform;
        }

        ~PaddingCryptoTransform()
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
            if (_disposed)
                return;

            _disposed = true;

            Transform?.Dispose();
        }

        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount,
            byte[] outputBuffer, int outputOffset)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            return Transform.TransformBlock(inputBuffer, inputOffset, inputCount, outputBuffer,
                outputOffset);
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            if (inputBuffer == null)
                throw new ArgumentNullException(nameof(inputBuffer));

            if (inputOffset < 0 || inputOffset > inputBuffer.Length)
                throw new ArgumentException("Offset is invalid.", nameof(inputOffset));

            if (inputCount < 0 || inputCount > inputBuffer.Length - inputOffset)
                throw new ArgumentException("Count is invalid.", nameof(inputCount));

            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            var blockSize = Transform.InputBlockSize;
            var block = new byte[inputCount / blockSize + blockSize];

            Buffer.BlockCopy(inputBuffer, inputOffset, block, 0, inputCount);

            var result = Transform.TransformFinalBlock(block, 0, block.Length);

            Array.Resize(ref result, inputCount);

            return result;
        }
    }
}
