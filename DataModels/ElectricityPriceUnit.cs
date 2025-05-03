using Newtonsoft.Json;

namespace Wattmate_Site.DataModels
{
    public class ElectricityPriceUnit
    {
        public int Id { get; set; }
        public string Zone { get; set; }

        [JsonProperty("DKK_per_kWh")]
        public float DKK { get; set; }

        [JsonProperty("EUR_per_kWh")]
        public float EUR { get; set; }

        [JsonProperty("EXR")]
        public float Exr { get; set; }
        [JsonProperty("time_start")]
        public DateTime TimeStart { get; set; }

        [JsonProperty("time_end")]
        public DateTime TimeEnd { get; set; }
    }
}
