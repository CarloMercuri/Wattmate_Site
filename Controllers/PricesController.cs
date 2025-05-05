using Microsoft.AspNetCore.Mvc;
using Wattmate_Site.DataProcessing;

namespace Wattmate_Site.Controllers
{
    public class PricesController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetPricesForDay(string day, string zone)
        {
            PricesProcessor p = new PricesProcessor();
            if (DateTime.TryParse(day, out DateTime date)) 
            {
                var prices = await p.GetDayPrices(date, zone);
                return Ok(prices);
            }
            else
            {
                return BadRequest("Wrong date format.");
            }

        }
    }
}
