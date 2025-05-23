using Wattmate_Site.DataModels.Devices;

namespace Wattmate_Site.Controllers.ViewModels
{
    public class ExtraTelemetryViewModel
    {
        public string DeviceId { get; set; }
        public FridgeDeviceData FridgeData { get; set; }
    }
}
