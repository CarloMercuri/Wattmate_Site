
using Microsoft.AspNetCore.Authentication;
using System.Data;
using Wattmate_Site.DataModels;
using Wattmate_Site.DataProcessing.Interfaces;
using Wattmate_Site.Security.Encryption;
using Wattmate_Site.Security.Models;
using Wattmate_Site.Users.Database;
using Wattmate_Site.Users.UserAuthentication.Interfaces;
using Wattmate_Site.Users.UserAuthentication.Models;
using Wattmate_Site.Utilities;
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
                //
                // Try and see if a user with the same email exists already.
                //

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

                // In case of error
                if (!encryptionResult.Success)
                {
                    return new UserCreationRequestResult()
                    {
                        Success = false,
                        Message = "Internal error"
                    };
                }

                // Create user and password entry
                UserModel m = new UserModel()
                {
                    UserName = loginData.UserEmail,
                    Name = loginData.Name,
                    Surname = loginData.Surname
                };

                DatabaseQueryResponse created = _db.InsertNewUser(m);

                if (!created.Success)
                {
                    return new UserCreationRequestResult()
                    {
                        Success = false,
                        Message = "Failed to create new user, internal error."
                    };
                }

                DatabaseQueryResponse passwordInserted = _db.InsertUpdatePassword(m.UserName,
                                                                                  encryptionResult.PasswordData.HashedPassword,
                                                                                  encryptionResult.PasswordData.Salt,
                                                                                  encryptionResult.PasswordData.Iterations,
                                                                                  encryptionResult.PasswordData.Algorithm);

                if (!passwordInserted.Success)
                {
                    return new UserCreationRequestResult()
                    {
                        Success = false,
                        Message = "User created, but failed to create new password entry"
                    };
                }


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


        public UserAuthenticationRequestResult AuthenticateUser(string userName, string password)
        {
            UserAuthenticationRequestResult _result = new UserAuthenticationRequestResult();
            _result.UserData = null;
            _result.Success = false;

            try
            {

                UsersDatabaseQueries _db = new UsersDatabaseQueries();
                DatabaseQueryResponse _pw = _db.FetchActiveUserPassword(userName);

                if (!_pw.Success || _pw.Data.Rows.Count == 0)
                {
                    return new UserAuthenticationRequestResult()
                    {
                        Success = false,
                        Message = "Internal error."
                    };
                }

                DataRow row = _pw.Data.Rows[0];
                PasswordHashData pData = new PasswordHashData();
                pData.HashedPassword = DBUtils.FetchAsString(row["password_hash"]);
                pData.Salt = DBUtils.FetchAsString(row["salt"]);
                pData.Algorithm = DBUtils.FetchAsString(row["hash_algorithm"]);
                pData.Iterations = DBUtils.FetchAsInt32(row["iterations"]);


                // Decrypt the input password and match it with the stored one
                IPasswordProcessor _crypto = AuthenticationManager.GetAppropriatePasswordProcessor(pData.Algorithm);
                bool encryptionResult = _crypto.VerifyPassword(password, pData.HashedPassword, pData.Salt, pData.Iterations);

                if (!encryptionResult)
                {
                    return new UserAuthenticationRequestResult()
                    {
                        Success = false,
                        Message = "Invalid username / password combination"
                    };
                }

                // SUCCESS

                var dbUser = _db.GetUserData(userName);

                return new UserAuthenticationRequestResult()
                {
                    Success = true,
                    UserData = dbUser.UserModel
                };



            }
            catch (Exception ex)
            {
                _result.Success = false;
                _result.Message = "Internal Error";
                return _result;
            }

        }

    }
}
