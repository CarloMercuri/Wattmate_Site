using System.Collections.Concurrent;
using System.Data;
using Wattmate_Site.Controllers.DeviceController;
using Wattmate_Site.DataModels;
using Wattmate_Site.Utilities;
using Wattmate_Site.WDatabase;

namespace Wattmate_Site.Devices
{
    public class DeviceProcessor
    {
  
        private string GetSecretKeyForDevice(string deviceId)
        {
            // 🔥 TODO: Replace with real lookup (from database, config, etc.)
            if (deviceId == "device-abc-123")
                return "your-very-secret-key-1234567890"; // must match Arduino's key

            return null;
        }

        public List<DeviceModel> GetUserDevices(string user_email)
        {
            WDatabaseQueries _db = new WDatabaseQueries();
            DatabaseQueryResponse dbData = _db.GetUserDevices(user_email);

            List<DeviceModel> devices = new();
            // Error
            if (!dbData.Success)
            {
                return null;
            }

            // Return null model in case of user not found
            if (dbData.Data.Rows.Count == 0)
            {
                return devices;
            }

            foreach (DataRow row in dbData.Data.Rows)
            {
                DeviceModel device = new DeviceModel();
                device.DeviceId = DBUtils.FetchAsString(row["device_id"]);
                device.DeviceName = DBUtils.FetchAsString(row["device_name"]);

                devices.Add(device);
            }

            return devices;
        }
    }
}
