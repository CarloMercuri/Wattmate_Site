using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using Wattmate_Site.Controllers.DeviceController;
using Wattmate_Site.Controllers.ViewModels;
using Wattmate_Site.DataProcessing;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Wattmate_Site.Controllers
{
    public class PricesController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> GetPricesForDay(string day, string zone)
        {
            PricesProcessor p = new PricesProcessor();
            if (DateTime.TryParse(day, out DateTime date)) 
            {
                var prices = await p.GetDaysPrices([date], zone);
                return Ok(prices);
            }
            else
            {
                return BadRequest("Wrong date format.");
            }

        }

        [HttpPost]
        public async Task<IActionResult> GetTodayPrices([FromBody] DevicePollRequest request)
        {
            PricesProcessor p = new PricesProcessor();
            var prices = await p.GetDaysPrices([DateTime.Now], "DK2");
            return Json(prices);

        }

        [HttpGet]
        public async Task<IActionResult> GetStandardPrices(string zone)
        {
            PricesProcessor p = new PricesProcessor();
            var prices = await p.GetDaysPrices([DateTime.Now, DateTime.Now.AddDays(1)], zone);
            return Json(prices);          

        }

        public async Task<IActionResult> Index()
        {
            List<DateTime> dates = new List<DateTime>();
            dates.Add(DateTime.Now);
            dates.Add(DateTime.Now.AddDays(1));


            PricesViewModel model = new();
            PricesProcessor _prices = new();

            model.Prices = await _prices.GetDaysPrices(dates, "DK2");

            return View(model);
        }
    }
}
