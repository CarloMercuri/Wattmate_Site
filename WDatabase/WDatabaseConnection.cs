using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Wattmate_Site.WDatabase
{
    public class WDatabaseConnection
    {
        string connectionString = "Server = mssql13.unoeuro.com; Database = wattmate_dk_db_main; User Id = wattmate_dk; Password = dchEwHaxk5R64FfBGpgn; MultipleActiveResultSets=True;";
        private SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

        /// <summary>
        /// Executes a query and returns a DatabaseQueryResponse a Success boolean, and data if there is any.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DatabaseQueryResponse SendQuery(string query, params SqlParameter[] parameters)
        {
            DatabaseQueryResponse response = new DatabaseQueryResponse();
            response.TimeStamp = DateTime.Now;
            SqlConnection _connection = GetConnection();

            try
            {
                using (SqlCommand command = new SqlCommand(query, _connection))
                {
                    foreach (SqlParameter parameter in parameters)
                        command.Parameters.Add(parameter);

                    _connection.Open();

                    DataTable dataTable = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(command))
                        da.Fill(dataTable);

                    _connection.Close();
                    response.Success = true;
                    response.Data = dataTable;


                    return response;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ResponseMessage = ex.Message;
                response.Exception = ex.InnerException;
                return response;
            }
            finally
            {
                _connection.Close();
                _connection.Dispose();
            }

        }

        /// <summary>
        /// Executes a non query and returns a DatabaseQueryResponse with a Success boolean, and affected rows if any.
        /// </summary>
        /// <param name="targetDatabase"></param>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DatabaseQueryResponse SendNonQuery(string query, params SqlParameter[] parameters)
        {
            DatabaseQueryResponse _response = new DatabaseQueryResponse();
            _response.TimeStamp = DateTime.Now;
            SqlConnection _connection = GetConnection();


            try
            {
                _connection.Open();

                using (SqlCommand cmd = new SqlCommand(query, _connection))
                {
                    foreach (SqlParameter parameter in parameters)
                        cmd.Parameters.Add(parameter);

                    _response.AffectedRows = cmd.ExecuteNonQuery();
                }

                _response.Success = true;
                return _response;
            }
            catch (Exception ex)
            {
                _response.Data = null;
                _response.ResponseMessage = ex.Message;
                _response.Exception = ex;
                return _response;
            }
            finally
            {
                _connection.Close();
                _connection.Dispose();

            }
        }


        public DatabaseQueryResponse CallStoredProcedure(string procedureName, params SqlParameter[] variables)
        {
            DatabaseQueryResponse _response = new DatabaseQueryResponse();
            _response.TimeStamp = DateTime.Now;
            SqlConnection _connection = GetConnection();

            try
            {
                _connection.Open();

                using (SqlCommand cmd = new SqlCommand(procedureName, _connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    foreach(SqlParameter p in variables)
                    {
                        cmd.Parameters.Add(p);
                    }
      
                    cmd.ExecuteNonQuery();
                }

                _response.Success = true;
                return _response;

            }
            catch (Exception ex)
            {
                _response.Data = null;
                _response.ResponseMessage = ex.Message;
                _response.Exception = ex;
                return _response;
            }
            finally
            {
                _connection.Close();
                _connection.Dispose();

            }
        }
    }
}
