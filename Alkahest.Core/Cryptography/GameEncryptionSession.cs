using System;
using System.Collections.Generic;
using System.Linq;

namespace Alkahest.Core.Cryptography
{
    public sealed class GameEncryptionSession
    {
        public static readonly int KeySize = 128;

        public Direction Direction { get; }

        public IReadOnlyList<byte> ClientKey1 => _clientKey1;

        public IReadOnlyList<byte> ClientKey2 => _clientKey2;

        public IReadOnlyList<byte> ServerKey1 => _serverKey1;

        public IReadOnlyList<byte> ServerKey2 => _serverKey2;

        public IReadOnlyList<byte> DecryptionKey => _decryptKey;

        public IReadOnlyList<byte> EncryptionKey => _encryptKey;

        readonly byte[] _clientKey1;

        readonly byte[] _clientKey2;

        readonly byte[] _serverKey1;

        readonly byte[] _serverKey2;

        readonly byte[] _decryptKey;

        readonly byte[] _encryptKey;

        readonly GameEncryption _decryptor;

        readonly GameEncryption _encryptor;

        static void CheckKey(byte[] key, string name)
        {
            if (key == null)
                throw new ArgumentNullException(name);

            if (key.Length != KeySize)
                throw new ArgumentException("Invalid key length.", name);
        }

        public GameEncryptionSession(Direction direction, byte[] clientKey1, byte[] clientKey2,
            byte[] serverKey1, byte[] serverKey2)
        {
            CheckKey(clientKey1, nameof(clientKey1));
            CheckKey(clientKey2, nameof(clientKey2));
            CheckKey(serverKey1, nameof(serverKey1));
            CheckKey(serverKey2, nameof(serverKey2));

            Direction = direction.CheckValidity(nameof(direction));

            _clientKey1 = clientKey1.ToArray();
            _clientKey2 = clientKey2.ToArray();
            _serverKey1 = serverKey1.ToArray();
            _serverKey2 = serverKey2.ToArray();

            _decryptKey = XorKey(ShiftKey(clientKey2, 29, false),
                XorKey(ShiftKey(serverKey1, 67, true), clientKey1));
            _decryptor = new GameEncryption(_decryptKey);

            var tmp = ShiftKey(serverKey2, 41, true);

            _decryptor.Apply(tmp, 0, tmp.Length);

            _encryptKey = tmp;
            _encryptor = new GameEncryption(_encryptKey);
        }

        static void CheckParameters(byte[] data, int offset, int length)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (offset < 0 || offset > data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));

            if (length < 0 || length > data.Length - offset)
                throw new ArgumentOutOfRangeException(nameof(length));
        }

        public void Decrypt(byte[] data, int offset, int count)
        {
            CheckParameters(data, offset, count);

            (Direction == Direction.ClientToServer ? _decryptor : _encryptor).Apply(data, offset, count);
        }

        public void Encrypt(byte[] data, int offset, int count)
        {
            CheckParameters(data, offset, count);

            (Direction == Direction.ClientToServer ? _encryptor : _decryptor).Apply(data, offset, count);
        }

        static byte[] XorKey(byte[] key1, byte[] key2)
        {
            return key1.Zip(key2, (a, b) => (byte)(a ^ b)).ToArray();
        }

        static byte[] ShiftKey(byte[] key, int shift, bool direction)
        {
            var result = new byte[key.Length];

            for (var i = 0; i < key.Length; i++)
            {
                if (direction)
                    result[(i + shift) % key.Length] = key[i];
                else
                    result[i] = key[(i + shift) % key.Length];
            }

            return result;
        }
    }
}
