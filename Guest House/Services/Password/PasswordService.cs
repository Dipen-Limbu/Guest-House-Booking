using System;
using System.Security.Cryptography;
using System.Text;

namespace Guest_House.Services.Password
{
    /// <summary>
    /// Implements PBKDF2 with HMAC-SHA256 password hashing and constant-time verification
    /// adhering to OWASP guidelines without requiring external NuGet packages.
    /// </summary>
    public class PasswordService : IPasswordService
    {
        private const int SaltSize = 16; // 128 bit cryptographic salt
        private const int KeySize = 32;  // 256 bit subkey
        private const int Iterations = 100000; // OWASP recommended minimum for PBKDF2
        private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;
        private const char Delimiter = ':';

        public string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Password cannot be empty.", nameof(password));
            }

            var salt = RandomNumberGenerator.GetBytes(SaltSize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                Iterations,
                Algorithm,
                KeySize);

            // Format: Base64(salt):Base64(hash):iterations
            return $"{Convert.ToBase64String(salt)}{Delimiter}{Convert.ToBase64String(hash)}{Delimiter}{Iterations}";
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(passwordHash))
            {
                return false;
            }

            var parts = passwordHash.Split(Delimiter);
            if (parts.Length != 3)
            {
                return false;
            }

            try
            {
                var salt = Convert.FromBase64String(parts[0]);
                var hash = Convert.FromBase64String(parts[1]);
                if (!int.TryParse(parts[2], out var iterations))
                {
                    return false;
                }

                var testHash = Rfc2898DeriveBytes.Pbkdf2(
                    Encoding.UTF8.GetBytes(password),
                    salt,
                    iterations,
                    Algorithm,
                    hash.Length);

                return CryptographicOperations.FixedTimeEquals(hash, testHash);
            }
            catch
            {
                return false;
            }
        }
    }
}
