using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;
using Wattmate_Site.Security.Models;

namespace Wattmate_Site.Security.Encryption
{
    public class Pbkdf2PasswordProcessor : IPasswordProcessor
    {
        int _defaultIterations = 10000;

        public NewPasswordHashResult HashNewPassword(string password)
        {
            NewPasswordHashResult re = new NewPasswordHashResult();
            try
            {
                byte[] salt = CreateNewSalt();
                re.Salt = Convert.ToHexString(salt);
                re.HashedPassword = HashPassword(password, salt, _defaultIterations);
                re.Iterations = _defaultIterations;

                re.Success = true;
                return re;
            }
            catch (Exception ex)
            {
                re.Success = false;
                re.Message = "Internal error";
                return re;
            }
          
        }

        private byte[] CreateNewSalt()
        {
            using var rng = RandomNumberGenerator.Create();
            const int SaltSize = 128 / 8; // 128 bits

            // Produce a version 2 text hash.
            byte[] salt = new byte[SaltSize];
            rng.GetBytes(salt);

            return salt;
        }

        public bool VerifyPassword(string inputPassword, string storedHash, string storedSalt, int iterations)
        {
            byte[] hashBytes = Convert.FromHexString(storedHash);
            byte[] saltBytes = Convert.FromHexString(storedSalt);

            string inputHashed = HashPassword(inputPassword, saltBytes, iterations);

            return CryptographicOperations.FixedTimeEquals(Convert.FromHexString(inputHashed), Convert.FromHexString(storedHash));
        }

        private string HashPassword(string password, byte[] salt, int iterations)
        {
            HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;
            const int keySize = 64;
            const KeyDerivationPrf Pbkdf2Prf = KeyDerivationPrf.HMACSHA1; // default for Rfc2898DeriveBytes

            var hash = Rfc2898DeriveBytes.Pbkdf2(
                                         Encoding.UTF8.GetBytes(password),
                                         salt,
                                         iterations,
                                         hashAlgorithm,
                                         keySize);

            return Convert.ToHexString(hash);
        }
    }
}
