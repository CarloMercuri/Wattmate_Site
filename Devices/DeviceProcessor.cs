using System.Collections.Concurrent;
using System.Data;
using Wattmate_Site.Controllers.DeviceController;
using Wattmate_Site.DataModels;
using Wattmate_Site.DataModels.DataTransferModels;
using Wattmate_Site.DataModels.Devices;
using Wattmate_Site.DataProcessing;
using Wattmate_Site.Utilities;
using Wattmate_Site.WDatabase;
using Wattmate_Site.WDatabase.Interfaces;
using Wattmate_Site.WDatabase.Queries;

namespace Wattmate_Site.Devices
{
    public class DeviceProcessor
    {
        public static Dictionary<string, DateTime> LastSeenDevices = new Dictionary<string, DateTime>();

        private IWDatabaseQueries _db { get; set; }

        public DeviceProcessor(IWDatabaseQueries db)
        {
            _db = db;
        }

        public static void UpdateLastSeenDevices(string deviceId)
        {
            if (LastSeenDevices.ContainsKey(deviceId)) 
            {
                LastSeenDevices[deviceId] = DateTime.Now;
            }
            else
            {
                LastSeenDevices.Add(deviceId, DateTime.Now);
            }
        }

        public void InsertNewTelemetry(TelemetryDataDTO reading)
        {
            _db.InsertNewTelemetry(reading);

        }

        public void UpdateDoorStatus(DeviceDoorStatus request)
        {
            _db.UpdateDoorStatus(request);
        }

        public void RequestDeviceStatuschange(DeviceStatus status)
        {
            string cmd = status.Status == "Active" ? "ACT_1" : "CLS_1";
            DeviceRequestsProcessor.SendCommand(new DeviceCommandRequest()
            {
                DeviceId = status.DeviceId,
                Command = cmd
            });
        }

 
        public void UpdateDeviceStatus(DeviceStatus status)
        {
            _db.UpdateDeviceStatus(status);
        }

        private string GetSecretKeyForDevice(string deviceId)
        {
            // 🔥 TODO: Replace with real lookup (from database, config, etc.)
            if (deviceId == "device-abc-123")
                return "your-very-secret-key-1234567890"; // must match Arduino's key

            return null;
        }

        public void ProcessDeviceTelemetry(TelemetryDataDTO reading)
        {
            TelemetryData fromDto = new TelemetryData()
            {
                DeviceId = reading.DeviceId,
                Timestamp = DateTime.Parse(reading.Timestamp),
                KwhReading = reading.KwhReading,
                Temperature = reading.Temperature,
                ReleActive = reading.ReleActive,
            };

            InsertNewTelemetry(reading);
            DatabaseQueryResponse dataResponse = _db.GetFridgeDeviceData(fromDto.DeviceId);
            if (!dataResponse.Success || dataResponse.Data.Rows.Count == 0) 
            {
                return;
            }

 
            FridgeDeviceData data = new FridgeDeviceData();

            data.MinimumTemperature = DBUtils.FetchAsFloat(dataResponse.Data.Rows[0]["min_temp"]);
            data.MaximumTemperature = DBUtils.FetchAsFloat(dataResponse.Data.Rows[0]["max_temp"]);
            data.CurrentTemperature = DBUtils.FetchAsFloat(dataResponse.Data.Rows[0]["current_temperature"]);
            data.TargetTemperature = DBUtils.FetchAsFloat(dataResponse.Data.Rows[0]["target_temp"]);
            data.AvarageRisePerMinute = DBUtils.FetchAsFloat(dataResponse.Data.Rows[0]["avarage_rise"]);
            data.AvarageFallPerMinute = DBUtils.FetchAsFloat(dataResponse.Data.Rows[0]["avarage_fall"]);

            // Return the request to the arduino, and then process in the background
            Task t1 = Task.Run(() => { CalculateFridgeStatus(fromDto.DeviceId, data); });

        }

        private async void CalculateFridgeStatus(string deviceId, FridgeDeviceData data)
        {
            float minTemp = data.MinimumTemperature;
            float maxTemp = data.MaximumTemperature;
            float currentTemp = data.CurrentTemperature;
            float targetTemp = data.TargetTemperature;
            float risePerMinute = data.AvarageRisePerMinute;
            float fallPerMinute = data.AvarageFallPerMinute;

            if (currentTemp < minTemp)
            {
                DeviceCommandRequest r = new DeviceCommandRequest()
                {
                    DeviceId = deviceId,
                    Command = "CLS_1"
                };
                DeviceRequestsProcessor.SendCommand(r);
                // TURN OFF
                return;
            }

            if (currentTemp > maxTemp)
            {
                DeviceCommandRequest r = new DeviceCommandRequest()
                {
                    DeviceId = deviceId,
                    Command = "ACT_1"
                };
                DeviceRequestsProcessor.SendCommand(r);
                // TURN ON
                return;
            }

            PricesProcessor pp = new PricesProcessor();
            List<ElectricityPriceUnit> prices = await pp.GetDaysPrices([DateTime.Now, DateTime.Now.AddDays(1)], "DK2");

            DateTime currentTime = DateTime.Now;
            int currentPriceIndex = 0;

            // Find the unit in the prices list that corresponds to NOW
            for (int i = 0; i < prices.Count; i++)
            {
                if (prices[i].TimeStart < currentTime) 
                {
                    currentPriceIndex = i;
                    break;
                }
            }

            int untilTooWarm = (int)(Math.Abs(maxTemp - currentTemp) / risePerMinute);
            int untillTooCold = (int)(Math.Abs(minTemp - currentTemp) / fallPerMinute);

            bool comingCheaper = false;

            // BETTER CALCUALATION OF HOURS (ex now if its 118 minutes, itll say 1 hour)
            for (int i = currentPriceIndex; i < prices.Count || i < (untilTooWarm / 60); i++) 
            {
                if (prices[i].DKK < prices[currentPriceIndex].DKK) 
                {
                    comingCheaper = true;
                }
            }

            if (comingCheaper) 
            {
                DeviceCommandRequest r = new DeviceCommandRequest()
                {
                    DeviceId = deviceId,
                    Command = "CLS_1"
                };
                DeviceRequestsProcessor.SendCommand(r);
                // TURN OFF
            }
            else
            {
                DeviceCommandRequest r = new DeviceCommandRequest()
                {
                    DeviceId = deviceId,
                    Command = "ACT_1"
                };
                DeviceRequestsProcessor.SendCommand(r);
                // TURN ON
            }

            return;

        }

        private void CalculateAvarages(string deviceId, FridgeDeviceData data)
        {
            return;

            DatabaseQueryResponse r = _db.GetFridgeTelemetry(deviceId, DateTime.Now.AddHours(-5), DateTime.Now);
            if (!r.Success || r.Data.Rows.Count == 0) return;

            List<TelemetryData> telemetry = new List<TelemetryData>();

            foreach(DataRow row in r.Data.Rows)
            {
                TelemetryData d = new TelemetryData();
                d.DeviceId = deviceId;
                d.Temperature = DBUtils.FetchAsFloat(row["temperature"]);
                d.Timestamp = DBUtils.FetchAsDateTime(row["timestamp"], DateTime.MinValue);
                telemetry.Add(d);
            }

            int index = 0;
            PeakTroughPoint lowestFound = null;
            PeakTroughPoint highestFound = null;
            float reading = telemetry[0].Temperature;
            bool rising = false;

            if (telemetry[0].Temperature < telemetry[1].Temperature)
                rising = true;
            else
                rising = false;

            for (int i = 1; i < telemetry.Count; i++) 
            {
                if (telemetry[i].Temperature < reading)
                {
                    reading = telemetry[i].Temperature;
                    if (rising)
                    {
                        highestFound = new PeakTroughPoint()
                        {
                            Time = telemetry[i].Timestamp,
                            Temperature = telemetry[i].Temperature,
                        };
                    }

                    rising = false;
                }
                else if (telemetry[i].Temperature > reading) 
                {
                    reading = telemetry[i].Temperature;
                    if (!rising)
                    {
                        lowestFound = new PeakTroughPoint()
                        {
                            Time = telemetry[i].Timestamp,
                            Temperature = telemetry[i].Temperature,
                        };
                    }

                    rising = true;
                }

                //if(highestFound && lowestFound)
                //{

                //}
            }
        }

        public List<DeviceModel> GetUserDevices(string user_email)
        {
            DatabaseQueryResponse dbData = _db.GetUserDevices(user_email);

            List<DeviceModel> devices = new();
            // Error
            if (!dbData.Success)
            {
                return null;
            }

            // Return null model in case of user not found
            if (dbData.Data.Rows.Count == 0)
            {
                return devices;
            }

            foreach (DataRow row in dbData.Data.Rows)
            {
                DeviceModel device = new DeviceModel();
                device.DeviceId = DBUtils.FetchAsString(row["device_id"]);
                device.DeviceName = DBUtils.FetchAsString(row["device_name"]);
                device.UserId = DBUtils.FetchAsString(row["user_id"]);
                device.Status = DBUtils.FetchAsString(row["status"]);
                device.DeviceType = DBUtils.FetchAsString(row["device_type"]);

                if (!LastSeenDevices.ContainsKey(device.DeviceId))
                {
                    device.Online = false;
                }
                else
                {
                    device.Online = DateTime.Now - LastSeenDevices[device.DeviceId] < new TimeSpan(0, 1, 0);
                }
               

                if(device.DeviceType == "FRIDGE_1")
                {
                    DatabaseQueryResponse r2 = _db.GetFridgeDeviceData(device.DeviceId);

                    device.FridgeDevice = new FridgeDeviceData();

                    if (r2.Success && r2.Data.Rows.Count > 0)
                    {
                        device.FridgeDevice.MinimumTemperature = DBUtils.FetchAsFloat(r2.Data.Rows[0]["min_temp"]);
                        device.FridgeDevice.MaximumTemperature = DBUtils.FetchAsFloat(r2.Data.Rows[0]["max_temp"]);
                        device.FridgeDevice.CurrentTemperature = DBUtils.FetchAsFloat(r2.Data.Rows[0]["current_temperature"]);
                        device.FridgeDevice.TargetTemperature = DBUtils.FetchAsFloat(r2.Data.Rows[0]["target_temp"]);
                    }

                }

                devices.Add(device);
            }



            return devices;
        }
    }
}
