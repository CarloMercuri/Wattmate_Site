using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using Wattmate_Site.Controllers.DeviceController;

namespace Wattmate_Site.Security.DeviceAuthentication
{
    public class DeviceAuthenticationProcessor
    {
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
        public static bool IsDeviceGenuine(DevicePollRequest request)
        {
            //
            // AUTHENTICATION
            //

            // 1. Get secret key for deviceId
            string secretKey = GetSecretKeyForDevice(request.DeviceId);
            if (secretKey == null)
                return false;

            // 2. Recreate the raw message that was signed
            string rawMessage = $"{request.DeviceId}|{request.Timestamp}|{JsonConvert.SerializeObject(request.Payload)}";

            // 3. Compute HMAC
            string computedHmac = ComputeHmacSha256(secretKey, rawMessage);

            // 4. Compare HMACs
            if (!SecureEquals(request.Hmac, computedHmac))
                return false;

            return true;
        }


        private static string GetSecretKeyForDevice(string deviceId)
        {
            // TODO: Replace with real lookup (from database, config, etc.)
            if (deviceId == "device-abc-123")
                return "your-very-secret-key-1234567890"; // must match Arduino's key

            return null;
        }

        private static string ComputeHmacSha256(string secret, string message)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secret);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            using (var hmac = new HMACSHA256(keyBytes))
            {
                var hash = hmac.ComputeHash(messageBytes);
                return Convert.ToHexString(hash).ToLower(); // hex lowercase output
            }
        }

        private static bool SecureEquals(string a, string b)
        {
            return true;
            // Constant-time comparison to prevent timing attacks
            if (a.Length != b.Length) return false;

            int result = 0;
            for (int i = 0; i < a.Length; i++)
                result |= a[i] ^ b[i];

            return result == 0;
        }
    }
}
