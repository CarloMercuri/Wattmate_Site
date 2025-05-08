using Microsoft.AspNetCore.Mvc;
using Wattmate_Site.WLog;

namespace Wattmate_Site.Controllers
{
    public class LogsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetLogs()
        {
            return Ok(WLogging.GetLogs());  
        }
    }
}
