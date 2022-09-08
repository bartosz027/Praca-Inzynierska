using System;
using System.Security.Cryptography;

namespace Network.Shared.Core {

    public static class EncryptionRSA {
        public static void Init() {
            _KeySize = 2048;
            _BlockSize = _KeySize / 8;
            _MaxDataSize = _BlockSize - 42;

            using (var rsa = new RSACryptoServiceProvider(_KeySize)) {
                PublicKey = rsa.ExportParameters(false);
                PrivateKey = rsa.ExportParameters(true);
            }
        }

        public static byte[] Encrypt(byte[] data, RSAParameters public_key) {
            var blocks = (int)Math.Ceiling((double)data.Length / (double)_MaxDataSize);
            var result = new byte[_BlockSize * blocks];

            using (var rsa = new RSACryptoServiceProvider(_KeySize)) {
                rsa.ImportParameters(public_key);

                for(int i = 0; i < blocks; i++) {
                    var block_offset = (i * _MaxDataSize);
                    var block_length = (data.Length - block_offset < _MaxDataSize) ? (data.Length - block_offset) : _MaxDataSize;

                    var block_data = new byte[block_length];
                    Array.Copy(data, block_offset, block_data, 0, block_data.Length);

                    var encrypted_block = rsa.Encrypt(block_data, true);
                    Array.Copy(encrypted_block, 0, result, i * _BlockSize, encrypted_block.Length);
                }
            }

            return result;
        }

        public static byte[] Decrypt(byte[] data) {
            var blocks = data.Length / _BlockSize;
            var result = new byte[0];

            using (var rsa = new RSACryptoServiceProvider(_KeySize)) {
                rsa.ImportParameters(PrivateKey);

                for (int i = 0; i < blocks; i++) {
                    var block_data = new byte[_BlockSize];
                    Array.Copy(data, i * _BlockSize, block_data, 0, block_data.Length);

                    var decrypted_block = rsa.Decrypt(block_data, true);
                    Array.Resize(ref result, result.Length + decrypted_block.Length);

                    var offset = result.Length - decrypted_block.Length;
                    Array.Copy(decrypted_block, 0, result, offset, decrypted_block.Length);
                }
            }

            return result;
        }

        // Keys
        public static RSAParameters PublicKey { get; private set; }
        public static RSAParameters PrivateKey { get; private set; }

        // Configuration
        private static int _KeySize, _BlockSize;
        private static int _MaxDataSize;
    }

}