using System;
using System.Security.Cryptography;

namespace QuanLyNhanVien.Security
{
    public static class PasswordHelper
    {
        // Configuration constants
        private const int SaltSize = 16; // 16 bytes = 128 bits
        private const int HashSize = 32; // 32 bytes = 256 bits
        private const int Iterations = 100_000; // PBKDF2 iterations

        public static void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Password cannot be empty", nameof(password));

            salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
            {
                hash = pbkdf2.GetBytes(HashSize);
            }
        }

        public static bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (storedHash == null || storedHash.Length == 0) return false;
            if (storedSalt == null || storedSalt.Length == 0) return false;

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, storedSalt, Iterations, HashAlgorithmName.SHA256))
            {
                var computed = pbkdf2.GetBytes(HashSize);
                return CryptographicOperations.FixedTimeEquals(computed, storedHash);
            }
        }

        public static bool NeedsRehash(int iterations) => iterations < Iterations;
    }
}
