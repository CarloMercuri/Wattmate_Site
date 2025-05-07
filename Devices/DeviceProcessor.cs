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
        public static Dictionary<string, DateTime> LastSeenDevices = new Dictionary<string, DateTime>();

        public static void UpdateLastSeenDevices(string deviceId)
        {
            if (LastSeenDevices.ContainsKey(deviceId)) 
            {
                LastSeenDevices[deviceId] = DateTime.Now;
            }
            else
            {
                LastSeenDevices.Add(deviceId, DateTime.Now);
            }
        }

        public void InsertNewKhwReading(KhwReading reading)
        {
            WDatabaseQueries _db = new();
            _db.InsertKhwReading(reading);
        }

        public void RequestDeviceStatuschange(DeviceStatus status)
        {
            string cmd = status.Status == "Active" ? "ACT_1" : "CLS_1";
            DeviceRequestsProcessor.SendCommand(new DeviceCommandRequest()
            {
                DeviceId = status.DeviceId,
                Command = cmd
            });
        }

 
        public void UpdateDeviceStatus(DeviceStatus status)
        {
            WDatabaseQueries _db = new();
            _db.UpdateDeviceStatus(status);
        }

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
                device.Status = DBUtils.FetchAsString(row["status"]);
                if (!LastSeenDevices.ContainsKey(device.DeviceId))
                {
                    device.Online = false;
                }
                else
                {
                    device.Online = DateTime.Now - LastSeenDevices[device.DeviceId] < new TimeSpan(0, 1, 0);
                }
                devices.Add(device);
            }



            return devices;
        }
    }
}
