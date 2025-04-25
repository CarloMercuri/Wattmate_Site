using Wattmate_Site.Users.UserAuthentication.Interfaces;
using Wattmate_Site.Users.UserAuthentication.Models;

namespace Wattmate_Site.Users.UserAuthentication.Processors
{
    public class LocalAuthenticationProcessor : IWattmateAuthenticationService
    {
        public UserAuthenticationRequestResult AuthenticateUser(string inputUserName, string password)
        {
            throw new NotImplementedException();
        }

        public UserCreationRequestResult CreateNewUser(UserLoginData data)
        {
            throw new NotImplementedException();
        }
    }
}
