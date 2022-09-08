using System.IO;
using System.Security.Cryptography;

namespace Network.Shared.Core {

    public class EncryptionAES {
        public EncryptionAES() {
            _Aes = Aes.Create();
            _Aes.Mode = CipherMode.CBC;

            _Aes.KeySize = 256;
            _Aes.BlockSize = 128;

            _Aes.GenerateKey();
            _Aes.GenerateIV();

            _Encryptor = _Aes.CreateEncryptor();
            _Decryptor = _Aes.CreateDecryptor();
        }

        public EncryptionAES(byte[] key, byte[] iv) {
            _Aes = Aes.Create();
            _Aes.Mode = CipherMode.CBC;

            _Aes.KeySize = 256;
            _Aes.BlockSize = 128;

            _Aes.Key = key;
            _Aes.IV = iv;

            _Encryptor = _Aes.CreateEncryptor();
            _Decryptor = _Aes.CreateDecryptor();
        }

        public byte[] Encrypt(byte[] data) {
            using (var ms = new MemoryStream()) {
                using (var cs = new CryptoStream(ms, _Encryptor, CryptoStreamMode.Write)) {
                    cs.Write(data, 0, data.Length);
                }

                return ms.ToArray();
            }
        }

        public byte[] Decrypt(byte[] data) {
            using (var ms = new MemoryStream()) {
                using (var cs = new CryptoStream(ms, _Decryptor, CryptoStreamMode.Write)) {
                    cs.Write(data, 0, data.Length);
                }

                return ms.ToArray();
            }
        }

        public byte[] GetKey() {
            return _Aes.Key;
        }

        public byte[] GetIV() {
            return _Aes.IV;
        }

        private Aes _Aes;
        private ICryptoTransform _Encryptor, _Decryptor;
    }

}