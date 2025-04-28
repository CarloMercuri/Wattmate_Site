using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using Wattmate_Site.Devices;

namespace Wattmate_Site.Controllers.DeviceController
{
    public class DeviceController : ControllerBase
    {

        

 
        [HttpPost]
        public async Task<IActionResult> Poll([FromBody] DevicePollRequest request)
        {
            //
            // AUTHENTICATION
            //

            //// 1. Get secret key for deviceId
            //string secretKey = GetSecretKeyForDevice(request.DeviceId);
            //if (secretKey == null)
            //    return Unauthorized("Unknown device");

            //// 2. Recreate the raw message that was signed
            //string rawMessage = $"{request.DeviceId}|{request.Timestamp}|{JsonConvert.SerializeObject(request.Payload)}";

            //// 3. Compute HMAC
            //string computedHmac = ComputeHmacSha256(secretKey, rawMessage);

            //// 4. Compare HMACs
            //if (!SecureEquals(request.Hmac, computedHmac))
            //    return Unauthorized("Invalid signature");

            // 5. If valid, process the request
            //
            // PROCESSING
            //

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
                return Ok(new DeviceCommandResponse { HasCommand = false });
            }
        }

        // Server (admin panel? mobile app?) triggers a command to a device
        [HttpPost()]
        public IActionResult SendCommand([FromBody] DeviceCommandRequest request)
        {
            if (DeviceRequestsProcessor.SendCommand(request))
            {
                return Ok("Command delivered to device!");
            }

            return NotFound("Device not polling right now.");
        }

    /*
     Arduino sends: 
        {
            "deviceId": "device-abc-123",
            "timestamp": "2025-04-26T14:20:00Z",
            "payload": {
                // anything else you want to send
            },
            "hmac": "abc1234efg5678..."  // <- HMAC of deviceId + timestamp + payload
        }
     */



        private string GetSecretKeyForDevice(string deviceId)
        {
            // TODO: Replace with real lookup (from database, config, etc.)
            if (deviceId == "device-abc-123")
                return "your-very-secret-key-1234567890"; // must match Arduino's key

            return null;
        }

        private string ComputeHmacSha256(string secret, string message)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secret);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            using (var hmac = new HMACSHA256(keyBytes))
            {
                var hash = hmac.ComputeHash(messageBytes);
                return Convert.ToHexString(hash).ToLower(); // hex lowercase output
            }
        }

        private bool SecureEquals(string a, string b)
        {
            // Constant-time comparison to prevent timing attacks
            if (a.Length != b.Length) return false;

            int result = 0;
            for (int i = 0; i < a.Length; i++)
                result |= a[i] ^ b[i];

            return result == 0;
        }
    }

    //
    // Models
    //

    public class DevicePollRequest
    {
        public string DeviceId { get; set; }
        public string Timestamp { get; set; }
        public object Payload { get; set; } // you can make this a specific type if you know it
        public string Hmac { get; set; }
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
    }

}
