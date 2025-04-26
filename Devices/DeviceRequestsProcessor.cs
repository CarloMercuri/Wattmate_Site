using System.Collections.Concurrent;
using Wattmate_Site.Controllers.DeviceController;

namespace Wattmate_Site.Devices
{
    public class DeviceRequestsProcessor
    {
        // Store waiting requests (deviceId -> pending response)
        public static ConcurrentDictionary<string, TaskCompletionSource<DeviceCommandResponse>> PendingRequests = new();

        public static TaskCompletionSource<DeviceCommandResponse> AddRequest(DevicePollRequest request)
        {
            var tcs = new TaskCompletionSource<DeviceCommandResponse>(TaskCreationOptions.RunContinuationsAsynchronously);
            PendingRequests[request.DeviceId] = tcs;
            return tcs;
        }

        public static bool RemoveRequest(DevicePollRequest request)
        {
            return PendingRequests.TryRemove(request.DeviceId, out _);
          
        }

        public static bool SendCommand(DeviceCommandRequest request)
        {
            if (PendingRequests.TryGetValue(request.DeviceId, out var tcs))
            {
                // Send the command back to waiting Arduino
                tcs.TrySetResult(new DeviceCommandResponse
                {
                    HasCommand = true,
                    Command = request.Command
                });

                return true;
            }

            return false;
        }
    }
}
