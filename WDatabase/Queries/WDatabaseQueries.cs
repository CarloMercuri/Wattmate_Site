using System.Data;
using Wattmate_Site.Controllers.DeviceController;
using Wattmate_Site.DataModels;
using Wattmate_Site.DataModels.DataTransferModels;
using Wattmate_Site.DataModels.Devices;
using Wattmate_Site.Utilities;
using Wattmate_Site.WDatabase.Interfaces;
using Wattmate_Site.WDatabase.QueriesModels;

namespace Wattmate_Site.WDatabase.Queries
{
    public class WDatabaseQueries : IWDatabaseQueries
    {
        WDatabaseConnection _connection;
        public WDatabaseQueries()
        {
            _connection = WDatabaseProcessor.GetDatabaseConnector();
        }



        public void UpdateDeviceStatus(DeviceStatus status)
        {
            _connection.CallStoredProcedure("UpdateDeviceStatus",
                                            DBUtils.AddSqlParameter("@DeviceId", status.DeviceId),
                                            DBUtils.AddSqlParameter("@Status", status.Status));
        }

        public DatabaseQueryResponse GetFridgeTelemetry(string device_id, DateTime start, DateTime end)
        {
            return _connection.CallStoredProcedureWithData("GetFridgeTelemetry",
                                                        DBUtils.AddSqlParameter("@device_id", device_id),
                                                        DBUtils.AddSqlParameter("@from", start),
                                                        DBUtils.AddSqlParameter("@to", end));
        }
  
        public DatabaseQueryResponse GetFridgeDeviceData(string deviceId)
        {
            return _connection.CallStoredProcedureWithData("GetFridgeDeviceSettings",
                                                        DBUtils.AddSqlParameter("@device_id", deviceId));
        }



        public DatabaseQueryResponse GetUserDevices(string email)
        {
            return _connection.CallStoredProcedureWithData("GetDevicesByUserEmail",
                                                    DBUtils.AddSqlParameter("@Email", email));
        }

        public DatabaseQueryResponse UpdateDoorStatus(DeviceDoorStatus request)
        {
            return _connection.CallStoredProcedure("InsertFridgeDoorStatus",
                                       DBUtils.AddSqlParameter("@device_id", request.DeviceId),
                                    DBUtils.AddSqlParameter("@timestamp", request.Timestamp),
                                    DBUtils.AddSqlParameter("@door_open", request.IsOpen));
        }

        public DatabaseQueryResponse InsertNewTelemetry(TelemetryDataDTO reading)
        {
            DateTime s = DateTime.MinValue;

            if (DateTime.TryParse(reading.Timestamp, out DateTime parsed))
            {
                _connection.CallStoredProcedure("InsertFridgeTelemetry",
                                   DBUtils.AddSqlParameter("@device_id", reading.DeviceId),
                                   DBUtils.AddSqlParameter("@timestamp", reading.Timestamp),
                                   DBUtils.AddSqlParameter("@temperature", reading.Temperature),
                                   DBUtils.AddSqlParameter("@rele_active", reading.ReleActive),
                                   DBUtils.AddSqlParameter("@kwh", reading.KwhReading));


                return new DatabaseQueryResponse()
                {
                    Success = true,
                };

            }
            else
            {
                return new DatabaseQueryResponse()
                {
                    Success = false,
                    ResponseMessage = "Invalid timestamp"
                };
            }
        }

        public DatabaseQueryResponse GetElectricityPrices(DateTime date, string zone)
        {
            //return _connection.CallStoredProcedureWithData("GetElectricityPricesByDate",
            //                                        DBUtils.AddSqlParameter("@TargetDate", date.ToString("yyyy-MM-dd")),
            //                                        DBUtils.AddSqlParameter("@Zone", zone));
            string query = @" SELECT *
                            FROM ElectricityPrices
                            WHERE 
                                CAST(time_start AT TIME ZONE 'UTC' AS DATE) = @TargetDate
	                        AND DK_ZONE = @Zone;";

            return _connection.SendQuery(query,
                                DBUtils.AddSqlParameter("@TargetDate", date.ToString("yyyy-MM-dd")),
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
