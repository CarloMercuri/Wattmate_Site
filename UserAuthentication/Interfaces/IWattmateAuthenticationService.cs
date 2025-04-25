using Wattmate_Site.UserAuthentication.Models;

namespace Wattmate_Site.UserAuthentication.Interfaces
{
    public interface IWattmateAuthenticationService
    {
        UserAuthenticationRequestResult AuthenticateUser(string inputUserName, string password);
    }
}
