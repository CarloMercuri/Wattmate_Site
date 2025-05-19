using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using Wattmate_Site.Controllers.DeviceController;
using Wattmate_Site.Utilities;
using Wattmate_Site.WDatabase;
using Wattmate_Site.WDatabase.Queries;

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
        public static bool IsDeviceGenuine(string deviceId, string timestamp, string clientHmac)
        {
            //
            // AUTHENTICATION
            //

            // 1. Get secret key for deviceId
            string secretKey = GetSecretKeyForDevice(deviceId);
            if (string.IsNullOrEmpty(secretKey))
                return false;

            // 2. Recreate the raw message that was signed
            string rawMessage = $"{deviceId}|{timestamp}|";

            // 3. Compute HMAC
            string computedHmac = ComputeHmacSha256(secretKey, rawMessage);

            // 4. Compare HMACs
            if (!SecureEquals(clientHmac, computedHmac))
                return false;

            return true;
        }


        private static string GetSecretKeyForDevice(string deviceId)
        {
            try
            {
                WDatabaseQueries _db = new();
                DatabaseQueryResponse r = _db.GetDeviceHmac(deviceId);

                if (!r.Success || r.Data.Rows.Count == 0)
                {
                    return "";
                }

                string hmac = DBUtils.FetchAsString(r.Data.Rows[0]["device_hmac"]);

                return hmac;
            }
            catch (Exception)
            {
                return "";
            }
           
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
            // Constant-time comparison to prevent timing attacks
            if (a.Length != b.Length) return false;

            int result = 0;
            for (int i = 0; i < a.Length; i++)
                result |= a[i] ^ b[i];

            return result == 0;
        }
    }
}
