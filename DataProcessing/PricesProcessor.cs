using Newtonsoft.Json;
using RestSharp;
using System.Data;
using System.Reflection.Metadata.Ecma335;
using System.Runtime;
using Wattmate_Site.DataModels;
using Wattmate_Site.Utilities;
using Wattmate_Site.WDatabase;
using Wattmate_Site.WDatabase.Queries;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Wattmate_Site.DataProcessing
{
    public class PricesProcessor
    {
        public async Task<List<ElectricityPriceUnit>> GetDaysPrices(List<DateTime> days, string zone)
        {
            List<ElectricityPriceUnit> returnData = new();
            WDatabaseQueries _db = new();

            foreach(DateTime day in days)
            {
                if(day.Date > DateTime.Now.Date)
                {
                    if(DateTime.Now.Hour < 14)
                    {
                        continue; // tomorrow prices are only available after 13.30
                    }
                }
                DatabaseQueryResponse resp = _db.GetElectricityPrices(day, zone);

                if (!resp.Success) return null;

                if (resp.Data.Rows.Count == 0)
                {
                    // try fetch for that day

                    await FetchDayPrices(day, zone);
                    resp = _db.GetElectricityPrices(day, zone);
                }

                foreach (DataRow row in resp.Data.Rows)
                {
                    var price = new ElectricityPriceUnit()
                    {
                        Id = DBUtils.FetchAsInt32(row["Id"]),
                        DKK = DBUtils.FetchAsFloat(row["DKK"]),
                        EUR = DBUtils.FetchAsFloat(row["EUR"]),
                        Zone = DBUtils.FetchAsString(row["DK_ZONE"]),
                        TimeStart = DBUtils.FetchAsDateTime(row["time_start"], DateTime.MinValue),
                        TimeEnd = DBUtils.FetchAsDateTime(row["time_end"], DateTime.MinValue),
                    };

                    // HORRIBLE FIX but I don't have time.
                    price.TimeStart = price.TimeStart.AddHours(-2);
                    price.TimeEnd = price.TimeEnd.AddHours(-2);
                    returnData.Add(price);
                }
            }

           

            return returnData;
            
        }

        // DK1 DK2 (dk2 = copenhagen)
        public async Task<bool> FetchDayPrices(DateTime date, string zone)
        {
            string day = date.Day < 10 ? "0" + date.Day : date.Day.ToString();
            string month = date.Month < 10 ? "0" + date.Month : date.Month.ToString();

            string url = $"https://www.elprisenligenu.dk/api/v1/prices/{date.Year.ToString()}/{month}-{day}_{zone}.json";
            RestClientOptions _clientOptions = new RestClientOptions(url);
            RestClient _client = new RestClient(_clientOptions);

            var request = new RestRequest();
            RestResponse r = await _client.ExecuteGetAsync(request);

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;

            List<ElectricityPriceUnit> data = new();

            if (r.StatusCode == System.Net.HttpStatusCode.OK)
            {
                data = JsonConvert.DeserializeObject<List<ElectricityPriceUnit>>(r.Content, jsonSettings);     
                foreach(var p in data)
                {
                    p.Zone = zone;
                }
            }
            else
            {
                return false;
            }

            // insert into db
            WDatabaseQueries _db = new();
            _db.InsertElectricityPrices(data);

            return true;

        }
    }
}
