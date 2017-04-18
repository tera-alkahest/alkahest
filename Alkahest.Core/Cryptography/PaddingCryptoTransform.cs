using System;
using System.Security.Cryptography;

namespace Alkahest.Core.Cryptography
{
    public sealed class PaddingCryptoTransform : ICryptoTransform
    {
        public int InputBlockSize => Transform.InputBlockSize;

        public int OutputBlockSize => Transform.OutputBlockSize;

        public bool CanTransformMultipleBlocks =>
            Transform.CanTransformMultipleBlocks;

        public bool CanReuseTransform => Transform.CanReuseTransform;

        public ICryptoTransform Transform { get; }

        public PaddingCryptoTransform(ICryptoTransform transform)
        {
            Transform = transform;
        }

        public void Dispose()
        {
            Transform.Dispose();
        }

        public int TransformBlock(byte[] inputBuffer, int inputOffset,
            int inputCount, byte[] outputBuffer, int outputOffset)
        {
            return Transform.TransformBlock(inputBuffer, inputOffset,
                inputCount, outputBuffer, outputOffset);
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset,
            int inputCount)
        {
            var blockSize = Transform.InputBlockSize;
            var block = new byte[inputCount / blockSize + blockSize];

            Buffer.BlockCopy(inputBuffer, inputOffset, block, 0, inputCount);

            var result = Transform.TransformFinalBlock(block, 0, block.Length);

            Array.Resize(ref result, inputCount);

            return result;
        }
    }
}
