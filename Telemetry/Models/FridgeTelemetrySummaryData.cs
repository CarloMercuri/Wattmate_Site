namespace Wattmate_Site.Telemetry.Models
{
    public class FridgeTelemetrySummaryData
    {
        public DateTime IntervalStart { get; set; }
        public DateTime IntervalEnd { get; set; }
        public float AvarageIntervalTemperature { get; set; }
        public int KwhDelta { get; set; }
        public float CostInDkk { get; set; }
    }
}
