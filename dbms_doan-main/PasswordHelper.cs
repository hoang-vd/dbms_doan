using System.Collections;
using System.Security.Cryptography;

namespace QuanLyNhanVien
{
    public static class PasswordHelper
    {
        public static void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                salt = new byte[16];
                rng.GetBytes(salt);
            }
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256))
            {
                hash = pbkdf2.GetBytes(32);
            }
        }

        public static bool VerifyPassword(string password, byte[] hash, byte[] salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256))
            {
                var testHash = pbkdf2.GetBytes(32);
                return StructuralComparisons.StructuralEqualityComparer.Equals(testHash, hash);
            }
        }
    }
}
