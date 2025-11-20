using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Commen.Helpers
{
    public class Helper
    {
        public static class PasswordHasher
        {
            // Hash password
            public static string HashPassword(string password, out string salt)
            {
                // توليد Salt عشوائي
                byte[] saltBytes = new byte[16];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(saltBytes);
                }
                salt = Convert.ToBase64String(saltBytes);

                // Hash باستخدام PBKDF2
                using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 100000, HashAlgorithmName.SHA256);
                byte[] hash = pbkdf2.GetBytes(32); // طول الهـاش 32 بايت
                return Convert.ToBase64String(hash);
            }

            // Verify password
            public static bool VerifyPassword(string password, string salt, string hash)
            {
                byte[] saltBytes = Convert.FromBase64String(salt);
                using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 100000, HashAlgorithmName.SHA256);
                byte[] hashBytes = pbkdf2.GetBytes(32);
                string newHash = Convert.ToBase64String(hashBytes);
                return newHash == hash;
            }
        }
        public static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

    }
}
