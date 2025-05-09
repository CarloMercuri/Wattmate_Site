namespace Wattmate_Site.Telemetry
{
    public class FridgeTelemetryRequest
    {
        public string DeviceId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int GroupingInterval { get; set; }
    }
}
