using System;
using System.Security.Cryptography;

namespace Network.Server.Core {

    // Source: https://stackoverflow.com/questions/4181198/how-to-hash-a-password
    internal static class PasswordHasher {
        public static string Hash(string password) {
            using (var rng = new RNGCryptoServiceProvider()) {
                var salt = new byte[_SaltSize];
                rng.GetBytes(salt);

                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, _Iterations)) {
                    var hash = pbkdf2.GetBytes(_HashSize);
                    var hash_bytes = new byte[_SaltSize + _HashSize];

                    Array.Copy(salt, 0, hash_bytes, 0, _SaltSize);
                    Array.Copy(hash, 0, hash_bytes, _SaltSize, _HashSize);

                    return Convert.ToBase64String(hash_bytes);
                }
            }
        }

        public static bool Verify(string password, string hashed_password) {
            var hash_bytes = Convert.FromBase64String(hashed_password);

            var salt = new byte[_SaltSize];
            Array.Copy(hash_bytes, 0, salt, 0, _SaltSize);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, _Iterations)) {
                var hash = pbkdf2.GetBytes(_HashSize);

                for (var i = 0; i < _HashSize; i++) {
                    if (hash_bytes[i + _SaltSize] != hash[i]) {
                        return false;
                    }
                }

                return true;
            }
        }

        // Configuration
        private const int _SaltSize = 16, _HashSize = 32;
        private const int _Iterations = 100000;
    }

}