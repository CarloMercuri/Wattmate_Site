using System.Data;
using Wattmate_Site.DataModels;
using Wattmate_Site.Telemetry.Models;
using Wattmate_Site.Utilities;
using Wattmate_Site.WDatabase;

namespace Wattmate_Site.Telemetry
{
    public class TelemetryProcessor
    {
        public float FindRelevantPrice(DateTime intervalStart, DateTime intervalEnd, List<ElectricityPriceUnit> prices)
        {
            var midpoint = intervalStart + TimeSpan.FromTicks((intervalEnd - intervalStart).Ticks / 2);

            // Find price interval containing the midpoint
            var matching = prices.FirstOrDefault(p => midpoint >= p.TimeStart && midpoint < p.TimeEnd);
            if (matching != null)
                return matching.DKK;

            // If no match, find closest by center distance
            return prices
                .OrderBy(p => Math.Abs((p.TimeStart.AddMinutes(30) - midpoint).Ticks))
                .FirstOrDefault().DKK;
        }

        public List<FridgeTelemetrySummaryData> GetFridgeTemperatureData(FridgeTelemetryRequest request)
        {
            TelemetryDatabaseQueries _db = new();
            DateTime parsedStart = DateTime.Parse(request.StartDate);
            DateTime parsedEnd = DateTime.Parse(request.EndDate);
            DatabaseQueryResponse response = null;
            if(request.GroupingInterval == 0)
            {
                response = _db.GetFridgeTelemetryFull(request.DeviceId,
                                                                           parsedStart,
                                                                           parsedEnd);
            }
            else
            {
                response = _db.GetFridgeTelemetrySummary(request.DeviceId,
                                                                           parsedStart,
                                                                           parsedEnd,
                                                                           request.GroupingInterval);
            }
       
            List<FridgeTelemetrySummaryData> returnData = new();

            if (!response.Success) return null;

            foreach(DataRow row in response.Data.Rows)
            {
                FridgeTelemetrySummaryData t = new FridgeTelemetrySummaryData();
                if(request.GroupingInterval == 0)
                {
                    t.IntervalStart = parsedStart;
                    t.IntervalEnd = parsedEnd;
                    t.AvarageIntervalTemperature = DBUtils.FetchAsFloat(row["Temperature"]);
                    t.KwhDelta = DBUtils.FetchAsInt32(row["KwhReading"]);
                }
                else
                {
                    t.IntervalStart = DBUtils.FetchAsDateTime(row["IntervalStart"], DateTime.MinValue);
                    t.IntervalEnd = t.IntervalStart.AddMinutes(request.GroupingInterval);
                    t.AvarageIntervalTemperature = DBUtils.FetchAsFloat(row["AvgTemperature"]);
                    t.KwhDelta = DBUtils.FetchAsInt32(row["TotalKwhDelta"]);
                }
 
                
             

                returnData.Add(t);
            }

            return returnData;
        }
    }
}
