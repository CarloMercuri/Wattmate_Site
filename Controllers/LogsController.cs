using Microsoft.AspNetCore.Mvc;
using Wattmate_Site.Controllers.DeviceController;
using Wattmate_Site.Devices;
using Wattmate_Site.Security.DeviceAuthentication;
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

        [HttpGet]
        public IActionResult GetQueue(string deviceId)
        {
            List<DeviceCommandResponse> actions = DeviceRequestsProcessor.GetRequests(new DevicePollRequest()
            {
                DeviceId = deviceId
            });
            return Ok(actions);
        }
    }
}
