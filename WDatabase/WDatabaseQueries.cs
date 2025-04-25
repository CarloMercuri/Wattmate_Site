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

       

    }
}
