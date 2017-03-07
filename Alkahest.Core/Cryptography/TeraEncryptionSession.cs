using System.Linq;

namespace Alkahest.Core.Cryptography
{
    public sealed class TeraEncryptionSession
    {
        public const int KeySize = 128;

        public Direction Direction { get; }

        public byte[] ClientKey1 => _clientKey1.ToArray();

        public byte[] ClientKey2 => _clientKey2.ToArray();

        public byte[] ServerKey1 => _serverKey1.ToArray();

        public byte[] ServerKey2 => _serverKey2.ToArray();

        public byte[] DecryptionKey => _decryptKey.ToArray();

        public byte[] EncryptionKey => _encryptKey.ToArray();

        readonly byte[] _clientKey1;

        readonly byte[] _clientKey2;

        readonly byte[] _serverKey1;

        readonly byte[] _serverKey2;

        readonly byte[] _decryptKey;

        readonly byte[] _encryptKey;

        readonly TeraEncryption _decryptor;

        readonly TeraEncryption _encryptor;

        public TeraEncryptionSession(Direction direction, byte[] clientKey1,
            byte[] clientKey2, byte[] serverKey1, byte[] serverKey2)
        {
            Direction = direction;

            _clientKey1 = clientKey1.ToArray();
            _clientKey2 = clientKey2.ToArray();
            _serverKey1 = serverKey1.ToArray();
            _serverKey2 = serverKey2.ToArray();

            var tmpKey1 = ShiftKey(serverKey1, 67, true);
            var tmpKey2 = XorKey(tmpKey1, clientKey1);
            tmpKey1 = ShiftKey(clientKey2, 29, false);

            _decryptKey = XorKey(tmpKey1, tmpKey2);
            _decryptor = new TeraEncryption(_decryptKey);
            
            tmpKey1 = ShiftKey(serverKey2, 41, true);

            _decryptor.Apply(tmpKey1, 0, tmpKey1.Length);

            _encryptKey = tmpKey1;
            _encryptor = new TeraEncryption(_encryptKey);
        }

        public void Decrypt(byte[] data, int offset, int length)
        {
            (Direction == Direction.ClientToServer ?
                _decryptor : _encryptor).Apply(data, offset, length);
        }

        public void Encrypt(byte[] data, int offset, int length)
        {
            (Direction == Direction.ClientToServer ?
                _encryptor : _decryptor).Apply(data, offset, length);
        }

        static byte[] XorKey(byte[] key1, byte[] key2)
        {
            var result = new byte[key1.Length];

            for (var i = 0; i < result.Length; i++)
                result[i] = (byte)(key1[i] ^ key2[i]);

            return result;
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
