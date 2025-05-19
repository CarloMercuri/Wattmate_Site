using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Wattmate_Site.Controllers.Attributes;
using Wattmate_Site.DataModels;
using Wattmate_Site.DataModels.DataTransferModels;
using Wattmate_Site.Devices;
using Wattmate_Site.WDatabase.Interfaces;
using Wattmate_Site.WLog;

namespace Wattmate_Site.Controllers.DeviceController
{
    public class DeviceController : SecureAccessController
    {

        DeviceProcessor _deviceProcessor;

        public DeviceController(IWDatabaseQueries db)
        {
                _deviceProcessor = new DeviceProcessor(db);
        }

        //[DeviceHmacAuthenticationRequired]
        [HttpPost]
        public async Task<IActionResult> GetStatus([FromBody] DevicePollRequest request)
        {
            DeviceProcessor.UpdateLastSeenDevices(request.DeviceId);

            List<DeviceCommandResponse> actions = DeviceRequestsProcessor.GetRequests(request);

            if(actions is null || actions.Count == 0)
            {
                return NoContent();
            }
            else
            {
                return Ok(actions);
            }
        }

        [DeviceHmacAuthenticationRequired]
        [HttpPost]
        public async Task<IActionResult> Poll([FromBody] DevicePollRequest request)
        {
           
            DeviceProcessor.UpdateLastSeenDevices(request.DeviceId);

            // Save it into pending requests
            var tcs = DeviceRequestsProcessor.AddRequest(request);

            // Wait for either a command, or timeout
            var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(30)));

            // After timeout or completion, clean up
            bool removed = DeviceRequestsProcessor.RemoveRequest(request);

            if (completedTask == tcs.Task)
            {
                // Command received
                return Ok(tcs.Task.Result);
            }
            else
            {
                // Timeout - no command
                return NoContent();
            }
        }

        [DeviceHmacAuthenticationRequired]
        [HttpPost]
        public IActionResult UpdateDoorStatus([FromBody] DeviceDoorStatus status)
        {
            try
            {
                _deviceProcessor.UpdateDoorStatus(status);
                return Ok();
            }
            catch (Exception ex)
            {
                return base.StatusCode((int)HttpStatusCode.InternalServerError, "Internal error");
                
            }
           
        }

        [AuthenticationRequired]
        [HttpPost]
        public IActionResult RequestChangeDeviceStatus([FromBody] DeviceStatus status)
        {
            _deviceProcessor.RequestDeviceStatuschange(status);
            return Ok();
        }

        
        [HttpPost]
        public async Task<IActionResult> Telemetry([FromBody] TelemetryDataDTO reading)
        {
            try
            {
                string obj = "";
                if (reading is null)
                {
                    obj = "null";
                }
                else
                {
                    obj = JsonConvert.SerializeObject(reading);
                }
                WLogging.Log($"SendKhwReading: reading object: " + obj);

                await _deviceProcessor.ProcessDeviceTelemetry(reading);
            }
            catch (Exception ex)
            {
                WLogging.Log($"TELEMETRY ERROR: " + ex.Message);
                return base.StatusCode((int)HttpStatusCode.InternalServerError);
            }
          


            
            return Ok();
        }

        [DeviceHmacAuthenticationRequired]
        [HttpPost]
        public IActionResult UpdateDeviceStatus([FromBody] DeviceStatus status)
        {
            _deviceProcessor.UpdateDeviceStatus(status);
            return Ok();
        }

        [LocalModeOnly]
        [HttpPost()]
        public IActionResult SendCommand([FromBody] DeviceCommandRequest request)
        {
            if (DeviceRequestsProcessor.SendCommand(request))
            {
                return Ok("Command delivered to device!");
            }

            return NotFound("Device not polling right now.");
        }

       
        

    }

    //
    // Models
    //

    public class DevicePollRequest
    {
        public string DeviceId { get; set; }
        public string Timestamp { get; set; }
        public string Payload { get; set; } // you can make this a specific type if you know it
        public string Hmac { get; set; }
    }

    public class DeviceDoorStatus
    {
        public string DeviceId { get; set; }
        public string Timestamp { get; set; }
        public bool IsOpen{ get; set; }
    }

    public class DeviceStatus
    {
        public string DeviceId { get; set; }
        public string Timestamp { get; set; }
        public string Status { get; set; }
    }

    public class DeviceCommandRequest
    {
        public string DeviceId { get; set; }
        public string Command { get; set; }
    }

    public class DeviceCommandResponse
    {
        public bool HasCommand { get; set; }
        public string Command { get; set; }
        public DateTime IssueDate { get; set; }
        // in seconds
        public uint Expirationtime { get; set; }
    }

}
