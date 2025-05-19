namespace Wattmate_Site.DataModels.DataTransferModels
{
    public class TelemetryDataDTO
    {
        public string DeviceId { get; set; }
        public string Timestamp { get; set; }
        public Int32 KwhReading { get; set; }
        public float Temperature { get; set; }
        public bool ReleActive { get; set; }
    }
}
