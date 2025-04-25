
using Microsoft.AspNetCore.Authentication;
using Wattmate_Site.DataModels;
using Wattmate_Site.DataProcessing.Interfaces;
using Wattmate_Site.Security.Encryption;
using Wattmate_Site.Security.Models;
using Wattmate_Site.Users.Database;
using Wattmate_Site.Users.UserAuthentication.Interfaces;
using Wattmate_Site.Users.UserAuthentication.Models;
using Wattmate_Site.WDatabase;
using Wattmate_Site.WDatabase.QueriesModels;

namespace Wattmate_Site.Users.UserAuthentication.Processors
{
    public class LiveAuthenticationProcessor : IWattmateAuthenticationService
    {

        public UserCreationRequestResult CreateNewUser(UserLoginData loginData)
        {
            try
            {
                UsersDatabaseQueries _db = new UsersDatabaseQueries();

                var dbUser = _db.GetUserData(loginData.UserEmail);
                if (dbUser.Result == DatabaseQueryResultCode.SystemError)
                {
                    return new UserCreationRequestResult()
                    {
                        Success = false,
                        Message = "Internal error"
                    };
                }

                if (dbUser.UserModel is not null)
                {
                    return new UserCreationRequestResult()
                    {
                        Success = false,
                        Message = "User already exists."
                    };
                }

                // Encrypt the password
                IPasswordProcessor _crypto = AuthenticationManager.GetCurrentPasswordProcessor();

                NewPasswordHashResult encryptionResult = _crypto.HashNewPassword(loginData.UserPassword);

                if (!encryptionResult.Success)
                {
                    return new UserCreationRequestResult()
                    {
                        Success = false,
                        Message = "Internal error"
                    };
                }

                UserModel m = new UserModel()
                {
                    UserEmail = loginData.UserEmail,
                    Name = loginData.Name,
                    Surname = loginData.Surname
                };

                bool created = _db.InsertNewUser(m);



                return new UserCreationRequestResult()
                {
                    Success = true
                };
            }
            catch (Exception ex)
            {
                string msg = "";
#if DEBUG
                msg = ex.Message;
#endif
                return new UserCreationRequestResult()
                {
                    Success = false,
                    Message = msg
                };
            }
          
        }


        public UserAuthenticationRequestResult AuthenticateUser(string inputUserName, string password)
        {
            UserAuthenticationRequestResult _result = new UserAuthenticationRequestResult();
            _result.AuthenticatedUserData = null;
            _result.Success = false;

            try
            {

                // Only need to be member of 1 of the groups to have access
                bool hasAccess = false;


                if (hasAccess)
                {
                    //_logSession.LogGeneral($"User '{samName} authorized. IsAdmin: {_result.AuthenticatedUserData.IsAdmin}. " +
                    //                       $"CanRead: {_result.AuthenticatedUserData.CanRead}. CanWrite: {_result.AuthenticatedUserData.CanWrite}",
                    //                       ProcessLogLevel.PRODUCTION);
                    _result.Success = hasAccess;
                    _result.Message = "User has permission.";

                    return _result;
                }
                else
                {
                    //_result.Success = false;
                    //_result.Message = "User does not have permission to access the page.";
                    //_logSession.LogGeneral($"Failed to authenticate user: {samName}", ProcessLogLevel.PRODUCTION);
                    //_result.AuthenticatedUserData = null;

                    return _result;
                }


            }
            catch (Exception ex)
            {

                _result.Message = "Could not verify permissions.";
                return _result;
            }

        }

    }
}
