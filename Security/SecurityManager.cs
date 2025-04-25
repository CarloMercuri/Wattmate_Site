using Wattmate_Site.Security.Encryption;

namespace Wattmate_Site.Security
{
    public class SecurityManager
    {
        public static IPasswordProcessor GetPasswordProcessor(string encryptionType)
        {
            switch (encryptionType)
            {
                case "PBKDF2":
                    return new Pbkdf2PasswordProcessor();

                default: return null;
            }
        }
    }
}
