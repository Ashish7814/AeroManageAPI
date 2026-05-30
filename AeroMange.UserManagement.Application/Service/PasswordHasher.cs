using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AeroMange.UserManagement.Application.Service
{
    public static class PasswordHasher
    {
        private const int SaltSize = 32;
        private const int HashSize = 32;
        private const int Iterations = 10000;

        public static (string Hash, string Salt) HashPassword(string password)
        {
            // Generate a random salt
            byte[] saltBytes = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }

            // Hash the password with the salt
            byte[] hashBytes = HashPasswordWithSalt(password, saltBytes);

            return (
                Convert.ToBase64String(hashBytes),
                Convert.ToBase64String(saltBytes)
            );
        }

        public static bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            byte[] saltBytes = Convert.FromBase64String(storedSalt);
            byte[] storedHashBytes = Convert.FromBase64String(storedHash);

            byte[] hashBytes = HashPasswordWithSalt(password, saltBytes);

            // Compare the computed hash with the stored hash
            return CryptographicOperations.FixedTimeEquals(hashBytes, storedHashBytes);
        }

        private static byte[] HashPasswordWithSalt(string password, byte[] salt)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256
            );

            return pbkdf2.GetBytes(HashSize);
        }
    }
}
