using System;

namespace Alkahest.Core.Cryptography
{
    public sealed class GameEncryptionSession : IDisposable
    {
        public static int KeySize => 128;

        public Direction Direction { get; }

        public ReadOnlyMemory<byte> ClientKey1 { get; }

        public ReadOnlyMemory<byte> ClientKey2 { get; }

        public ReadOnlyMemory<byte> ServerKey1 { get; }

        public ReadOnlyMemory<byte> ServerKey2 { get; }

        public ReadOnlyMemory<byte> DecryptionKey { get; }

        public ReadOnlyMemory<byte> EncryptionKey { get; }

        readonly GameEncryption _decryptor;

        readonly GameEncryption _encryptor;

        bool _disposed;

        public GameEncryptionSession(Direction direction, ReadOnlyMemory<byte> clientKey1,
            ReadOnlyMemory<byte> clientKey2, ReadOnlyMemory<byte> serverKey1,
            ReadOnlyMemory<byte> serverKey2)
        {
            static void CheckKey(ReadOnlyMemory<byte> key, string name)
            {
                if (key.Length != KeySize)
                    throw new ArgumentException("Invalid key length.", name);
            }

            CheckKey(clientKey1, nameof(clientKey1));
            CheckKey(clientKey2, nameof(clientKey2));
            CheckKey(serverKey1, nameof(serverKey1));
            CheckKey(serverKey2, nameof(serverKey2));

            Direction = direction.CheckValidity(nameof(direction));

            ClientKey1 = clientKey1.ToArray();
            ClientKey2 = clientKey2.ToArray();
            ServerKey1 = serverKey1.ToArray();
            ServerKey2 = serverKey2.ToArray();

            static Memory<byte> XorKey(ReadOnlyMemory<byte> key1, ReadOnlyMemory<byte> key2)
            {
                var result = new byte[key1.Length];

                for (var i = 0; i < result.Length; i++)
                    result[i] = (byte)(key1.Span[i] ^ key2.Span[i]);

                return result;
            }

            static Memory<byte> ShiftKey(ReadOnlyMemory<byte> key, int shift, bool direction)
            {
                var result = new byte[key.Length];

                for (var i = 0; i < result.Length; i++)
                {
                    if (direction)
                        result[(i + shift) % key.Length] = key.Span[i];
                    else
                        result[i] = key.Span[(i + shift) % key.Length];
                }

                return result;
            }

            DecryptionKey = XorKey(ShiftKey(clientKey2, 29, false),
                XorKey(ShiftKey(serverKey1, 67, true), clientKey1));
            _decryptor = new GameEncryption(DecryptionKey);

            var encKey = ShiftKey(serverKey2, 41, true);

            _decryptor.Apply(encKey);

            EncryptionKey = encKey;
            _encryptor = new GameEncryption(encKey);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            _decryptor.Dispose();
            _encryptor.Dispose();
        }

        public void Decrypt(Memory<byte> data)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            (Direction == Direction.ClientToServer ? _decryptor : _encryptor).Apply(data);
        }

        public void Encrypt(Memory<byte> data)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            (Direction == Direction.ClientToServer ? _encryptor : _decryptor).Apply(data);
        }
    }
}
