using Wattmate_Site.Controllers.DeviceController;
using Wattmate_Site.DataModels;
using Wattmate_Site.WDatabase.Interfaces;

namespace Wattmate_Site.WDatabase.Queries
{
    public class WLocalFileQueries : IWDatabaseQueries
    {
        public DatabaseQueryResponse GetElectricityPrices(DateTime date, string zone)
        {
            throw new NotImplementedException();
        }

        public DatabaseQueryResponse GetUserDevices(string email)
        {
            throw new NotImplementedException();
        }

        public bool InsertElectricityPrices(List<ElectricityPriceUnit> prices)
        {
            throw new NotImplementedException();
        }

        public DatabaseQueryResponse InsertNewTelemetry(TelemetryData reading)
        {
            throw new NotImplementedException();
        }

        public void UpdateDeviceStatus(DeviceStatus status)
        {
            throw new NotImplementedException();
        }

        public DatabaseQueryResponse UpdateDoorStatus(DeviceDoorStatus request)
        {
            throw new NotImplementedException();
        }
    }
}
