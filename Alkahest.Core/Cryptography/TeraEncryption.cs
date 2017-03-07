using System;
using System.Linq;

namespace Alkahest.Core.Cryptography
{
    internal sealed class TeraEncryption
    {
        sealed class TeraEncryptionKey
        {
            public int Size { get; set; }

            public uint[] Buffer { get; }

            public int Key { get; set; }

            public int Position1 { get; set; }

            public int Position2 { get; set; }

            public uint Sum { get; set; }

            public TeraEncryptionKey(int size, int position2)
            {
                Size = size;
                Buffer = new uint[size * sizeof(uint)];
                Position2 = position2;
            }
        }

        readonly TeraEncryptionKey[] _keys = new[]
        {
            new TeraEncryptionKey(55, 31),
            new TeraEncryptionKey(57, 50),
            new TeraEncryptionKey(58, 39)
        };

        int _changeData;

        int _changeLength;

        public TeraEncryption(byte[] key)
        {
            GenerateKey(key);
        }

        void GenerateKey(byte[] key)
        {
            // FIXME: Use BinaryReader instead of BitConverter here.

            var buf = new byte[_keys.Aggregate(0, (acc, x) => acc + x.Size) * sizeof(uint)];

            buf[0] = 128;

            for (var i = 1; i < buf.Length; i++)
                buf[i] = key[i % 128];

            for (var i = 0; i < buf.Length; i += TeraEncryptionHash.HashSize)
            {
                var sha = TeraEncryptionHash.ComputeHash(buf);

                for (var j = 0; j < sha.Length; j++)
                    Buffer.BlockCopy(BitConverter.GetBytes(sha[j]), 0,
                        buf, i + j * sizeof(uint), sizeof(uint));
            }

            // TODO: What's the deal with the magic indices into buf?

            for (var i = 0; i < _keys[0].Buffer.Length; i += sizeof(uint))
                _keys[0].Buffer[i / sizeof(uint)] =
                    BitConverter.ToUInt32(buf, i);

            for (var i = 0; i < _keys[1].Buffer.Length; i += sizeof(uint))
                _keys[1].Buffer[i / sizeof(uint)] =
                    BitConverter.ToUInt32(buf, 220 + i);

            for (var i = 0; i < _keys[2].Buffer.Length; i += sizeof(uint))
                _keys[2].Buffer[i / sizeof(uint)] =
                    BitConverter.ToUInt32(buf, 448 + i);
        }

        void DoRound()
        {
            var result = _keys[0].Key & _keys[1].Key | _keys[2].Key &
                (_keys[0].Key | _keys[1].Key);

            for (var j = 0; j < _keys.Length; j++)
            {
                var key = _keys[j];

                if (result == key.Key)
                {
                    var t1 = key.Buffer[key.Position1];
                    var t2 = key.Buffer[key.Position2];
                    var t3 = t1 <= t2 ? t1 : t2;

                    key.Sum = t1 + t2;
                    key.Key = t3 > key.Sum ? 1 : 0;
                    key.Position1 = (key.Position1 + 1) % key.Size;
                    key.Position2 = (key.Position2 + 1) % key.Size;
                }
            }
        }

        public void Apply(byte[] data, int offset, int length)
        {
            // TODO: Figure out the significance of the magic numbers here.

            var pre = length < _changeLength ? length : _changeLength;

            if (pre != 0)
            {
                for (var i = 0; i < pre; i++)
                    data[offset + i] ^= (byte)(_changeData >> (8 * (4 - _changeLength + i)));

                _changeLength -= pre;
            }

            for (var i = pre; i < length - 3; i += sizeof(uint))
            {
                DoRound();

                foreach (var key in _keys)
                {
                    data[offset + i] ^= (byte)key.Sum;
                    data[offset + i + 1] ^= (byte)(key.Sum >> 8);
                    data[offset + i + 2] ^= (byte)(key.Sum >> 16);
                    data[offset + i + 3] ^= (byte)(key.Sum >> 24);
                }
            }

            var remain = (length - pre) & 3;

            if (remain != 0)
            {
                DoRound();

                _changeData = 0;

                foreach (var key in _keys)
                    _changeData ^= (int)key.Sum;

                for (var i = 0; i < remain; i++)
                    data[offset + length - remain + i] ^= (byte)(_changeData >> (i * 8));

                _changeLength = 4 - remain;
            }
        }
    }
}
