using System;
using System.Security.Cryptography;

namespace Alkahest.Core.Cryptography
{
    public sealed class GameSHA1 : SHA1
    {
        bool _computed;

        ulong _lengthHigh;

        ulong _lengthLow;

        readonly byte[] _messageBlock = new byte[64];

        int _blockIndex;

        readonly uint[] _messageDigest;

        public GameSHA1()
        {
            _messageDigest = new uint[HashSize / 8 / sizeof(uint)];

            Initialize();
        }

        public override void Initialize()
        {
            _computed = false;
            _lengthHigh = 0;
            _lengthLow = 0;
            _blockIndex = 0;

            Array.Clear(_messageBlock, 0, _messageBlock.Length);

            _messageDigest[0] = 0x67452301;
            _messageDigest[1] = 0xefcdab89;
            _messageDigest[2] = 0x98badcfe;
            _messageDigest[3] = 0x10325476;
            _messageDigest[4] = 0xc3d2e1f0;
        }

        void ProcessMessageBlock()
        {
            var w = new uint[80];

            for (var t = 0; t < _messageBlock.Length / sizeof(uint); t++)
                w[t] = Bits.Compose(
                    ((uint)_messageBlock[t * 4], 24, 8),
                    (_messageBlock[t * 4 + 1], 16, 8),
                    (_messageBlock[t * 4 + 2], 8, 8),
                    (_messageBlock[t * 4 + 3], 0, 8));

            for (var i = 16; i < w.Length; i++)
                w[i] = w[i - 3] ^ w[i - 8] ^ w[i - 14] ^ w[i - 16];

            var a = _messageDigest[0];
            var b = _messageDigest[1];
            var c = _messageDigest[2];
            var d = _messageDigest[3];
            var e = _messageDigest[4];

            static uint RotateLeft(uint word, int bits)
            {
                return word << bits | word >> 8 * sizeof(uint) - bits;
            }

            for (var i = 0; i < w.Length; i++)
            {
                var temp = RotateLeft(a, 5) + e + w[i];

                if (i < 20)
                    temp += (b & c | ~b & d) + 0x5a827999;
                else if (i < 40)
                    temp += (b ^ c ^ d) + 0x6ed9eba1;
                else if (i < 60)
                    temp += (b & c | b & d | c & d) + 0x8f1bbcdc;
                else if (i < 80)
                    temp += (b ^ c ^ d) + 0xca62c1d6;

                e = d;
                d = c;
                c = RotateLeft(b, 30);
                b = a;
                a = temp;
            }

            _messageDigest[0] += a;
            _messageDigest[1] += b;
            _messageDigest[2] += c;
            _messageDigest[3] += d;
            _messageDigest[4] += e;

            _blockIndex = 0;
        }

        void PadMessage()
        {
            _messageBlock[_blockIndex++] = 0x80;

            if (_blockIndex > 56)
            {
                while (_blockIndex < 64)
                    _messageBlock[_blockIndex++] = 0;

                ProcessMessageBlock();
            }

            while (_blockIndex < 56)
                _messageBlock[_blockIndex++] = 0;

            _messageBlock[56] = (byte)Bits.Extract(_lengthHigh, 24, 8);
            _messageBlock[57] = (byte)Bits.Extract(_lengthHigh, 16, 8);
            _messageBlock[58] = (byte)Bits.Extract(_lengthHigh, 8, 8);
            _messageBlock[59] = (byte)Bits.Extract(_lengthHigh, 0, 8);

            _messageBlock[60] = (byte)Bits.Extract(_lengthLow, 24, 8);
            _messageBlock[61] = (byte)Bits.Extract(_lengthLow, 16, 8);
            _messageBlock[62] = (byte)Bits.Extract(_lengthLow, 8, 8);
            _messageBlock[63] = (byte)Bits.Extract(_lengthLow, 0, 8);

            ProcessMessageBlock();
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            foreach (var b in array.Slice(ibStart, cbSize))
            {
                _messageBlock[_blockIndex++] = b;

                _lengthLow += 8;

                if (_lengthLow == 0)
                    _lengthHigh++;

                if (_blockIndex == _messageBlock.Length)
                    ProcessMessageBlock();
            }
        }

        protected override byte[] HashFinal()
        {
            if (!_computed)
            {
                PadMessage();
                _computed = true;
            }

            var hash = new byte[HashSize / 8];

            for (int i = 0, j = 0; i < _messageDigest.Length; i++, j += sizeof(uint))
            {
                var val = _messageDigest[i];

                hash[j + 3] = (byte)Bits.Extract(val, 24, 8);
                hash[j + 2] = (byte)Bits.Extract(val, 16, 8);
                hash[j + 1] = (byte)Bits.Extract(val, 8, 8);
                hash[j] = (byte)Bits.Extract(val, 0, 8);
            }

            return hash;
        }
    }
}
