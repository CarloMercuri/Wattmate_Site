using System.Data;
using Wattmate_Site.DataModels;
using Wattmate_Site.Utilities;
using Wattmate_Site.WDatabase;
using Wattmate_Site.WDatabase.QueriesModels;

namespace Wattmate_Site.Users.Database
{
    public class UsersDatabaseQueries
    {
        WDatabaseConnection _connection;
        public UsersDatabaseQueries()
        {
            _connection = WDatabaseProcessor.GetDatabaseConnector();
        }

        public UserModelDatabaseRequest GetUserData(string email)
        {
            try
            {
                string query = $"SELECT * FROM Users WHERE email = @email";
                UserModelDatabaseRequest returnData = new UserModelDatabaseRequest();
                DatabaseQueryResponse response = _connection.SendQuery(query, DBUtils.AddSqlParameter("@email", email));

                // Error
                if (!response.Success)
                {
                    returnData.Result = DatabaseQueryResultCode.SystemError;
                    returnData.Message = "Error fetching data from database";
                    returnData.UserModel = null;
                    return returnData;
                }

                // Return null model in case of user not found
                if (response.Data.Rows.Count == 0)
                {
                    returnData.Result = DatabaseQueryResultCode.Success;
                    returnData.UserModel = null;
                    return returnData;
                }

                UserModel model = new UserModel();
                DataRow row = response.Data.Rows[0];
                model.UserEmail = DBUtils.FetchAsString(row["email"]);
                model.Name = DBUtils.FetchAsString(row["name"]);
                model.Surname = DBUtils.FetchAsString(row["surname"]);

                returnData.Result = DatabaseQueryResultCode.Success;
                returnData.UserModel = model;
                return returnData;
            }
            catch (Exception ex)
            {
                return new UserModelDatabaseRequest()
                {
                    Result = DatabaseQueryResultCode.SystemError,
                    Message = "Error parsing DB Data"
                };
            }

        }

        public DatabaseQueryResponse FetchActiveUserPassword(string email)
        {
            return _connection.CallStoredProcedureWithData("GetActiveUserPassword", DBUtils.AddSqlParameter("@user_email", email));
        }
        
        public DatabaseQueryResponse InsertNewUser(UserModel model)
        {
 
            return _connection.CallStoredProcedure("CreateUser", DBUtils.AddSqlParameter("@Email", model.UserEmail),
                                                            DBUtils.AddSqlParameter("@Name", model.Name),
                                                            DBUtils.AddSqlParameter("@Surname", model.Surname));

        }

        public DatabaseQueryResponse InsertUpdatePassword(string email, string passwordHash, string salt, int iterations, string algorithm)
        {
            return _connection.CallStoredProcedure("InsertUserPassword",
                                                     DBUtils.AddSqlParameter("@user_email", email),
                                                     DBUtils.AddSqlParameter("@password_hash", passwordHash),
                                                     DBUtils.AddSqlParameter("@salt", salt),
                                                     DBUtils.AddSqlParameter("@hash_algorithm", algorithm),
                                                     DBUtils.AddSqlParameter("@iterations", iterations));
        }
    }
}
