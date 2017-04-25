using System;
using System.Linq;

namespace Alkahest.Core.Cryptography
{
    sealed class TeraEncryption
    {
        sealed class KeyBlockGenerator
        {
            public int Size { get; }

            public uint Value { get; private set; }

            public bool InOverflow { get; private set; }

            readonly uint[] _key;

            int _positionA;

            int _positionB;

            public KeyBlockGenerator(int size, int positionB)
            {
                Size = size;
                _key = new uint[size];
                _positionB = positionB;
            }

            public void SetKey(byte[] buffer, int offset)
            {
                Buffer.BlockCopy(buffer, offset * sizeof(uint),
                    _key, 0, Size * sizeof(uint));
            }

            public void Advance()
            {
                var a = _key[_positionA++ % Size];
                var b = _key[_positionB++ % Size];

                Value = a + b;
                InOverflow = Math.Min(a, b) > Value;
            }
        }

        readonly KeyBlockGenerator[] _generators = new[]
        {
            new KeyBlockGenerator(55, 31),
            new KeyBlockGenerator(57, 50),
            new KeyBlockGenerator(58, 39)
        };

        uint _keyBlock;

        byte _remaining;

        public TeraEncryption(byte[] key)
        {
            var buf = new byte[_generators.Aggregate(0,
                (acc, x) => acc + x.Size) * sizeof(uint)];

            buf[0] = (byte)key.Length;

            for (var i = 1; i < buf.Length; i++)
                buf[i] = key[i % key.Length];

            using (var sha = new SHA1Tera())
            {
                for (var i = 0; i < buf.Length; i += sha.HashSize / 8)
                {
                    var hash = sha.ComputeHash(buf);

                    Buffer.BlockCopy(hash, 0, buf, i, hash.Length);
                }
            }

            var offset = 0;

            foreach (var gen in _generators)
            {
                gen.SetKey(buf, offset);
                offset += gen.Size;
            }
        }

        public void Apply(byte[] data, int offset, int length)
        {
            for (var i = 0; i < length; i++)
            {
                if (_remaining == 0)
                {
                    var states = _generators.Select(x =>
                        (gen: x, overflow: x.InOverflow));
                    var overflowed = states.Count(x => x.overflow) >= 2;

                    foreach (var (gen, overflow) in states)
                        if (overflow == overflowed)
                            gen.Advance();

                    _keyBlock = _generators.Aggregate(0U, (acc, x) => acc ^ x.Value);
                    _remaining = 4;
                }

                data[offset + i] = (byte)(data[offset + i] ^ _keyBlock);
                _keyBlock >>= 8;
                _remaining--;
            }
        }
    }
}
