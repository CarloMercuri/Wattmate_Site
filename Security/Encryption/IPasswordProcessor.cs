using System.Security.Cryptography;
using Wattmate_Site.Security.Models;

namespace Wattmate_Site.Security.Encryption
{
    public interface IPasswordProcessor
    {
        public NewPasswordHashResult HashNewPassword(string password);
        public bool VerifyPassword(string inputPassword, string storedHash, string storedSalt, int iterations);
    }
}
