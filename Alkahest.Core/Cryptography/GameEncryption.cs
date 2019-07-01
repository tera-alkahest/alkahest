using System;
using System.Buffers;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Alkahest.Core.Cryptography
{
    sealed class GameEncryption : IDisposable
    {
        sealed class KeyBlockGenerator : IDisposable
        {
            public ReadOnlyMemory<uint> Key => _key;

            public uint Value { get; private set; }

            public bool InOverflow { get; private set; }

            readonly Memory<uint> _key;

            readonly MemoryHandle _handle;

            int _positionA;

            int _positionB;

            public KeyBlockGenerator(int size, int positionB)
            {
                _key = new uint[size];
                _handle = _key.Pin();
                _positionB = positionB;
            }

            ~KeyBlockGenerator()
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
                _handle.Dispose();
            }

            public int SetKey(ReadOnlyMemory<byte> key)
            {
                key.Slice(0, _key.Length * sizeof(uint)).Span.CopyTo(_key.Span.AsBytes());

                return _key.Length;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public unsafe void Advance()
            {
                ref uint r = ref Unsafe.AsRef<uint>(_handle.Pointer);

                var a = Unsafe.Add(ref r, _positionA++ % _key.Length);
                var b = Unsafe.Add(ref r, _positionB++ % _key.Length);

                Value = a + b;
                InOverflow = Math.Min(a, b) > Value;
            }
        }

        readonly KeyBlockGenerator[] _generators = new[]
        {
            new KeyBlockGenerator(55, 31),
            new KeyBlockGenerator(57, 50),
            new KeyBlockGenerator(58, 39),
        };

        uint _keyBlock;

        byte _remaining;

        public GameEncryption(ReadOnlyMemory<byte> key)
        {
            var buf = new byte[_generators.Aggregate(0, (acc, x) => acc + x.Key.Length) * sizeof(uint)];

            buf[0] = (byte)key.Length;

            for (var i = 1; i < buf.Length; i++)
                buf[i] = key.Span[i % key.Length];

            using var sha = new GameSHA1();

            for (var i = 0; i < buf.Length; i += sha.HashSize / 8)
            {
                var hash = sha.ComputeHash(buf);

                Buffer.BlockCopy(hash, 0, buf, i, hash.Length);
            }

            var offset = 0;

            foreach (var gen in _generators)
                offset += gen.SetKey(buf.AsMemory().Slice(offset * sizeof(uint)));
        }

        public void Dispose()
        {
            foreach (var gen in _generators)
                gen.Dispose();
        }

        public void Apply(Memory<byte> data)
        {
            var span = data.Span;
            var length = data.Length;

            for (var i = 0; i < length; i++)
            {
                if (_remaining == 0)
                {
                    var overflows = 0;

                    foreach (var gen in _generators)
                        if (gen.InOverflow)
                            overflows++;

                    var overflowed = overflows >= 2;

                    _keyBlock = 0;

                    foreach (var gen in _generators)
                    {
                        if (gen.InOverflow == overflowed)
                            gen.Advance();

                        _keyBlock ^= gen.Value;
                    }

                    _remaining = 4;
                }

                span[i] ^= (byte)_keyBlock;

                _keyBlock >>= 8;
                _remaining--;
            }
        }
    }
}
