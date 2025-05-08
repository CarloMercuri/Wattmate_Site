namespace Wattmate_Site.DataModels
{
    public class TelemetryData
    {
        public string DeviceId { get; set; }
        public string Timestamp { get; set; }
        public Int32 KwhReading { get; set; }
        public float Temperature { get; set; }
        public bool ReleActive { get; set; }
     }
}
