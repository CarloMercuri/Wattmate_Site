using Wattmate_Site.Security.Encryption;

namespace Wattmate_Site.Users.UserAuthentication
{
    public class AuthenticationManager
    {
        public static IPasswordProcessor GetCurrentPasswordProcessor()
        {
            return new Pbkdf2PasswordProcessor();
        }
    }
}
