using System.Security.Cryptography;

namespace DoAn4.Helper
{
    public class PasswordHelper
    {
        private const int SaltSize = 64;
        private const int HashSize = 64;
        private const int Iterations = 10000;

        public static byte[] GenerateSalt()
        {
            var salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        public static byte[] HashPassword(string password, byte[] salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
            {
                return pbkdf2.GetBytes(HashSize);
            }
        }

        public static byte[] HashPassword(string password)
        {
            var salt = GenerateSalt();
            return HashPassword(password, salt);
        }

        public static bool VerifyPassword(string password, byte[] salt, byte[] hash)
        {
            var stringHash = HashPassword(password, salt);
            return SlowEquals(stringHash, hash);
        }

        private static bool SlowEquals(byte[] a, byte[] b)
        {
            uint diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= (uint)(a[i] ^ b[i]);
            }
            return diff == 0;
        }
    }
}
