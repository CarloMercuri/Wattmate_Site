using Wattmate_Site.UserAuthentication.Interfaces;
using Wattmate_Site.UserAuthentication.Models;

namespace Wattmate_Site.UserAuthentication.Processors
{
    public class LocalAuthenticationProcessor : IWattmateAuthenticationService
    {
        public UserAuthenticationRequestResult AuthenticateUser(string inputUserName, string password)
        {
            UserAuthenticationRequestResult _result = new UserAuthenticationRequestResult();
            _result.AuthenticatedUserData = null;
            _result.Success = false;

            // The username we need to check vs AD needs to be in email format, ending in @ff.local
            // To be user friendly, we allow the user to enter the username anyway they want, and we 
            // fix it here. 
            string userName = inputUserName.Split('@')[0];
            string userNameOnly = userName;
            userName += "@ff.local";

            if (inputUserName == "error@er.ro")
            {
                _result.Success = false;
                _result.Message = "Test error";
                return _result;
            }


            bool hasAccess = false;

            _result.AuthenticatedUserData = new AuthenticatedUserData()
            {
                UserEmail = userName,
                UserSamName = userName.Split("@")[0],
            };

            switch (userNameOnly.ToLower())
            {
                case "admin":
                    _result.AuthenticatedUserData.IsAdmin = true;
                    _result.AuthenticatedUserData.CanRead = true;
                    _result.AuthenticatedUserData.CanWrite = true;
                    _result.AuthenticatedUserData.UserRights = AuthenticatedUserRights.Admin;
                    hasAccess = true;
                    break;

                case "readonly":
                    _result.AuthenticatedUserData.CanRead = true;
                    _result.AuthenticatedUserData.UserRights = AuthenticatedUserRights.Read;
                    hasAccess = true;
                    break;

                case "norights":
                    break;

                default:
                    _result.AuthenticatedUserData.CanRead = true;
                    _result.AuthenticatedUserData.CanWrite = true;
                    _result.AuthenticatedUserData.UserRights = AuthenticatedUserRights.ReadWrite;
                    hasAccess = true;
                    break;
            }

            if (hasAccess)
            {
                _result.Success = hasAccess;
                _result.Message = "User has permission.";
              
                return _result;
            }
            else
            {
                _result.Success = false;
                _result.Message = "User does not have permission to access the page.";
                _result.AuthenticatedUserData = null;

                return _result;
            }
        }
    }
}
