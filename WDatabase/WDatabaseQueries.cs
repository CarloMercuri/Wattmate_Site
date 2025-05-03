using System.Data;
using Wattmate_Site.DataModels;
using Wattmate_Site.Utilities;
using Wattmate_Site.WDatabase.QueriesModels;

namespace Wattmate_Site.WDatabase
{
    public class WDatabaseQueries
    {
        WDatabaseConnection _connection;
        public WDatabaseQueries()
        {
            _connection = WDatabaseProcessor.GetDatabaseConnector();
        }
               
      
        public DatabaseQueryResponse GetUserDevices(string email)
        {
            return _connection.CallStoredProcedureWithData("GetDevicesByUserEmail", 
                                                    DBUtils.AddSqlParameter("@Email", email));
        }

        public DatabaseQueryResponse GetElectricityPrices(DateTime date, string zone)
        {
            return _connection.CallStoredProcedureWithData("GetElectricityPricesByDate",
                                                    DBUtils.AddSqlParameter("@TargetDate", date),
                                                    DBUtils.AddSqlParameter("@Zone", zone));
        }

        public bool InsertElectricityPrices(List<ElectricityPriceUnit> prices)
        {
            foreach (var pr in prices)
            {
                _connection.CallStoredProcedure("InsertElectricityPrice",
                                            DBUtils.AddSqlParameter("@DKK", pr.DKK),
                                            DBUtils.AddSqlParameter("@EUR", pr.EUR),
                                            DBUtils.AddSqlParameter("@DK_ZONE", pr.Zone),
                                            DBUtils.AddSqlParameter("@EXR", pr.Exr),
                                            DBUtils.AddSqlParameter("@time_start", pr.TimeStart),
                                            DBUtils.AddSqlParameter("@time_end", pr.TimeEnd));
            }

            return true;
        }
    }
}
