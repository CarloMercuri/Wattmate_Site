using Wattmate_Site.Users.UserAuthentication.Models;

namespace Wattmate_Site.Users.UserAuthentication.Interfaces
{
    public interface IWattmateAuthenticationService
    {
        UserAuthenticationRequestResult AuthenticateUser(string inputUserName, string password);
        UserCreationRequestResult CreateNewUser(UserLoginData data);
    }
}
