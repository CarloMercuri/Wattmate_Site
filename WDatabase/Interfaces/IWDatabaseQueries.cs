using Wattmate_Site.Controllers.DeviceController;
using Wattmate_Site.DataModels;
using Wattmate_Site.DataModels.DataTransferModels;

namespace Wattmate_Site.WDatabase.Interfaces
{
    public interface IWDatabaseQueries
    {
        void UpdateDeviceStatus(DeviceStatus status);

        DatabaseQueryResponse GetUserDevices(string email);
        DatabaseQueryResponse GetFridgeDeviceData(string deviceId);
        DatabaseQueryResponse GetFridgeTelemetry(string device_id, DateTime start, DateTime end);

        DatabaseQueryResponse UpdateDoorStatus(DeviceDoorStatus request);

        DatabaseQueryResponse InsertNewTelemetry(TelemetryDataDTO reading);

        DatabaseQueryResponse GetElectricityPrices(DateTime date, string zone);

        bool InsertElectricityPrices(List<ElectricityPriceUnit> prices);
    }
}
