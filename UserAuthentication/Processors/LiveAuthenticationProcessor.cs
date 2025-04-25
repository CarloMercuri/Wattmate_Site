
using Microsoft.AspNetCore.Authentication;
using Wattmate_Site.DataProcessing.Interfaces;
using Wattmate_Site.UserAuthentication.Interfaces;
using Wattmate_Site.UserAuthentication.Models;

namespace Wattmate_Site.UserAuthentication.Processors
{
    public class LiveAuthenticationProcessor : IWattmateAuthenticationService
    {
        //ISystemDataHandler _dataHandler;

        //public LiveAuthenticationProcessor(ISystemDataHandler dataHandler)
        //{
        //    _dataHandler = dataHandler;
        //}

        public UserCreationRequestResult CreateNewUser(UserLoginData loginData)
        {
            return null;
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
