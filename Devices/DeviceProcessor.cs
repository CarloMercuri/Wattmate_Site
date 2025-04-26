using System.Collections.Concurrent;
using Wattmate_Site.Controllers.DeviceController;

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
    }
}
