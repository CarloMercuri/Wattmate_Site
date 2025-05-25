using Microsoft.AspNetCore.Mvc;
using Wattmate_Site.Controllers.Attributes;
using Wattmate_Site.Controllers.ViewModels;
using Wattmate_Site.DataModels;
using Wattmate_Site.DataProcessing;
using Wattmate_Site.Devices;
using Wattmate_Site.Telemetry;
using Wattmate_Site.Telemetry.Models;
using Wattmate_Site.Users.UserAuthentication.Extensions;

namespace Wattmate_Site.Controllers
{
    public class TelemetryController : SecureAccessController
    {
        [AuthenticationRequired]
        [HttpPost]
        public async Task<IActionResult> GetFridgeTelemetry([FromBody] FridgeTelemetryRequest request)
        {
            try
            {
                PricesProcessor _prices = new();
                DateTime startDate = DateTime.MinValue;
                DateTime endDate = DateTime.MinValue;
                if (!DateTime.TryParse(request.StartDate, out startDate)) return BadRequest("Start Date invalid");
                if (!DateTime.TryParse(request.EndDate, out endDate)) return BadRequest("End Date invalid");

                ViewModels.TelemetryData model = new ViewModels.TelemetryData();

                // Get a list of unique days
                List<DateTime> dates = new List<DateTime>();
                for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    dates.Add(date);
                }
                model.Prices = await _prices.GetDaysPrices(dates, "DK2");

                TelemetryProcessor processor = new TelemetryProcessor();
                model.TemperatureData = processor.GetFridgeTemperatureData(request);

                float totalCost = 0;

                foreach (var interval in model.TemperatureData)
                {
                    float price = processor.FindRelevantPrice(interval.IntervalStart, interval.IntervalEnd, model.Prices);
                    float cost = price * (interval.KwhDelta / 1000f);
                    interval.CostInDkk = cost;
                    totalCost += cost;
                }

                model.TotalCost = totalCost;

                return Ok(model);
            }
            catch (Exception ex)
            {

                throw;
            }
          
        }

        [AuthenticationRequired]
        [HttpPost]
        public async Task<IActionResult> GetFridgeTelemetryBarKwh([FromBody] FridgeTelemetryRequest request)
        {
            try
            {
                PricesProcessor _prices = new();
                DateTime startDate = DateTime.MinValue;
                DateTime endDate = DateTime.MinValue;
                if (!DateTime.TryParse(request.StartDate, out startDate)) return BadRequest("Start Date invalid");
                if (!DateTime.TryParse(request.EndDate, out endDate)) return BadRequest("End Date invalid");

                ViewModels.TelemetryData model = new ViewModels.TelemetryData();

                // Get a list of unique days
                List<DateTime> dates = new List<DateTime>();
                for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    dates.Add(date);
                }
                model.Prices = await _prices.GetDaysPrices(dates, "DK2");

                TelemetryProcessor processor = new TelemetryProcessor();
                model.TemperatureData = processor.GetFridgeTemperatureData(request);

                float totalCost = 0;

                foreach (var interval in model.TemperatureData)
                {
                    float price = processor.FindRelevantPrice(interval.IntervalStart, interval.IntervalEnd, model.Prices);
                    float cost = price * (interval.KwhDelta / 1000f);
                    interval.CostInDkk = cost;
                    totalCost += cost;

                    if (interval.KwhDelta > 0) interval.KwhDelta = 4;
                }

                model.TotalCost = totalCost;

                return Ok(model);
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        [AuthenticationRequired]
        public IActionResult Index(string deviceId)
        {
            TelemetryViewModel model = new();
            model.DeviceId = deviceId;
            return View(model);
        }
    }
}
