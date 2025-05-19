namespace Wattmate_Site.DataModels.Devices
{
    public class FridgeDeviceData
    {
        public float CurrentTemperature { get; set; }
        public float TargetTemperature { get; set; }
        public float MinimumTemperature { get; set; }
        public float MaximumTemperature { get; set; }
        public float AvarageRisePerMinute { get; set; }
        public float AvarageFallPerMinute { get; set; }
    }
}
