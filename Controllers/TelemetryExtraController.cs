using Microsoft.AspNetCore.Mvc;
using Wattmate_Site.Controllers.Attributes;
using Wattmate_Site.Controllers.ViewModels;
using Wattmate_Site.Devices;
using Wattmate_Site.WDatabase.Interfaces;

namespace Wattmate_Site.Controllers
{
    public class TelemetryExtraController : SecureAccessController
    {

        DeviceProcessor _deviceProcessor;

        public TelemetryExtraController(IWDatabaseQueries db)
        {
            _deviceProcessor = new DeviceProcessor(db);
        }

        [AuthenticationRequired]
        public IActionResult Index(string deviceId)
        {
            ExtraTelemetryViewModel model = new();
            model.DeviceId = deviceId;

            model.FridgeData = _deviceProcessor.GetFridgeData(deviceId);

            return View(model);
        }

        [HttpPost]
        public IActionResult SaveTempData([FromBody] TempDataRequest request)
        {
            bool success = _deviceProcessor.SaveTempData(request);
            return Ok();
        }

        [HttpPost]
        public IActionResult SaveVariables([FromBody] VariablesRequest request)
        {
            bool success = _deviceProcessor.SaveVariables(request);
            return Ok();
        }

        [HttpPost]
        public IActionResult SaveVariables([FromBody] VariablesCalculationRequest request)
        {
            _deviceProcessor.CalculateVariables(request);
            return Ok();
        }

    }

    public class VariablesCalculationRequest
    {
        public string TimePointA { get; set; }
        public string TimePointB { get; set; }
    }

    public class TempDataRequest
    {
        public string DeviceId { get; set; }
        public float MinTemp { get; set; }
        public float MaxTemp { get; set; }
        public float TargetTemp { get; set; }
    }

    public class VariablesRequest
    {
        public string DeviceId { get; set; }
        public float FallRatio { get; set; }
        public float RiseRatio { get; set; }
    }
}
