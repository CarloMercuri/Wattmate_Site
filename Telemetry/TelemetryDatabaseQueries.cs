using Wattmate_Site.Utilities;
using Wattmate_Site.WDatabase;

namespace Wattmate_Site.Telemetry
{
    public class TelemetryDatabaseQueries
    {
        WDatabaseConnection _connection;
        public TelemetryDatabaseQueries()
        {
            _connection = WDatabaseProcessor.GetDatabaseConnector();
        }

        public DatabaseQueryResponse GetFridgeTelemetryFull(string _deviceId, DateTime _start, DateTime _end)
        {
            return _connection.CallStoredProcedureWithData("GetFridgeTemperatureFull",
                                                     DBUtils.AddSqlParameter("@DeviceId", _deviceId),
                                                     DBUtils.AddSqlParameter("@StartTime", _start),
                                                     DBUtils.AddSqlParameter("@EndTime", _end));
        }

        public DatabaseQueryResponse GetFridgeTelemetrySummary(string _deviceId, DateTime _start, DateTime _end, int _interval)
        {
           return _connection.CallStoredProcedureWithData("GetFridgeTelemetrySummary",
                                                    DBUtils.AddSqlParameter("@DeviceId", _deviceId),
                                                    DBUtils.AddSqlParameter("@StartTime", _start),
                                                    DBUtils.AddSqlParameter("@EndTime", _end),
                                                    DBUtils.AddSqlParameter("@IntervalMinutes", _interval));
        }
    }
}
