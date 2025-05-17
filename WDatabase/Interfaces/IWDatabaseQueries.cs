using Wattmate_Site.Controllers.DeviceController;
using Wattmate_Site.DataModels;

namespace Wattmate_Site.WDatabase.Interfaces
{
    public interface IWDatabaseQueries
    {
        void UpdateDeviceStatus(DeviceStatus status);

        DatabaseQueryResponse GetUserDevices(string email);

        DatabaseQueryResponse UpdateDoorStatus(DeviceDoorStatus request);

        DatabaseQueryResponse InsertNewTelemetry(TelemetryData reading);

        DatabaseQueryResponse GetElectricityPrices(DateTime date, string zone);

        bool InsertElectricityPrices(List<ElectricityPriceUnit> prices);
    }
}
