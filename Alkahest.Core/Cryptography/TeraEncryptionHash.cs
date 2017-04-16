namespace Alkahest.Core.Cryptography
{
    static class TeraEncryptionHash
    {
        // TODO: Do we really need this?

        sealed class TeraSHA1
        {
            const int BlockSize = 64;

            bool _computed;

            ulong _lengthHigh;

            ulong _lengthLow;

            readonly byte[] _messageBlock = new byte[BlockSize];

            int _blockIndex;

            readonly uint[] _messageDigest =
            {
                0x67452301,
                0xefcdab89,
                0x98badcfe,
                0x10325476,
                0xc3d2e1f0
            };

            public void Update(byte[] data)
            {
                foreach (var b in data)
                {
                    _messageBlock[_blockIndex++] = b;

                    _lengthLow += sizeof(byte) * 8;

                    if (_lengthLow == 0)
                        _lengthHigh++;

                    if (_blockIndex == BlockSize)
                        ProcessMessageBlock();
                }
            }

            public uint[] Hash()
            {
                if (!_computed)
                {
                    PadMessage();
                    _computed = true;
                }

                return _messageDigest;
            }

            static uint RotateLeft(uint word, int bits)
            {
                return word << bits | word >> sizeof(uint) * 8 - bits;
            }

            void ProcessMessageBlock()
            {
                var w = new uint[80];

                for (var t = 0; t < BlockSize / 4; t++)
                    w[t] = (uint)_messageBlock[t * 4] << 24 |
                        (uint)_messageBlock[t * 4 + 1] << 16 |
                        (uint)_messageBlock[t * 4 + 2] << 8 |
                        _messageBlock[t * 4 + 3];

                for (var t = 16; t < 80; t++)
                    w[t] = w[t - 3] ^ w[t - 8] ^ w[t - 14] ^ w[t - 16];

                var a = _messageDigest[0];
                var b = _messageDigest[1];
                var c = _messageDigest[2];
                var d = _messageDigest[3];
                var e = _messageDigest[4];

                for (var t = 0; t < 80; t++)
                {
                    var temp = RotateLeft(a, 5) + e + w[t];

                    if (t < 20)
                        temp += (b & c | ~b & d) + 0x5a827999;
                    else if (t < 40)
                        temp += (b ^ c ^ d) + 0x6ed9eba1;
                    else if (t < 60)
                        temp += (b & c | b & d | c & d) + 0x8f1bbcdc;
                    else if (t < 80)
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

                _messageBlock[56] = (byte)(_lengthHigh >> 24);
                _messageBlock[57] = (byte)(_lengthHigh >> 16);
                _messageBlock[58] = (byte)(_lengthHigh >> 8);
                _messageBlock[59] = (byte)_lengthHigh;

                _messageBlock[60] = (byte)(_lengthLow >> 24);
                _messageBlock[61] = (byte)(_lengthLow >> 16);
                _messageBlock[62] = (byte)(_lengthLow >> 8);
                _messageBlock[63] = (byte)_lengthLow;

                ProcessMessageBlock();
            }
        }

        public const int HashSize = 20;

        public static uint[] ComputeHash(byte[] data)
        {
            var sha = new TeraSHA1();

            sha.Update(data);

            return sha.Hash();
        }
    }
}
