using Wattmate_Site.DataModels;
using Wattmate_Site.Telemetry.Models;

namespace Wattmate_Site.Controllers.ViewModels
{
    public class TelemetryData
    {
        public List<ElectricityPriceUnit> Prices { get; set; } = new List<ElectricityPriceUnit>();
        public List<FridgeTelemetrySummaryData> TemperatureData { get; set; } = new List<FridgeTelemetrySummaryData>();
        public List<IntervalCostData> IntervalsData { get; set; } = new();
        public float TotalCost { get; set; }
    }
}
