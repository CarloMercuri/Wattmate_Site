using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Wattmate_Site.Controllers.DeviceController;

namespace Wattmate_Site.Devices
{
    public class DeviceRequestsProcessor
    {
        // Store waiting requests (deviceId -> pending response)
        public static ConcurrentDictionary<string, TaskCompletionSource<List<DeviceCommandResponse>>> PendingRequests = new();

        // Store waiting commands if device was not polling when the command was issued
        public static ConcurrentDictionary<string, ConcurrentQueue<DeviceCommandResponse>> CommandsQueue = new();

        /// <summary>
        /// Adds a new poll request for a device. 
        /// If there are queued commands for the device, they are immediately returned.
        /// </summary>
        /// <param name="request">The device's poll request information.</param>
        /// <returns>A TaskCompletionSource that will be completed when commands are available.</returns>
        public static TaskCompletionSource<List<DeviceCommandResponse>> AddRequest(DevicePollRequest request)
        {
            var tcs = new TaskCompletionSource<List<DeviceCommandResponse>>(TaskCreationOptions.RunContinuationsAsynchronously);

            // Add or overwrite the pending request for the device
            PendingRequests[request.DeviceId] = tcs;

            // If there are already commands queued for this device, deliver them immediately
            if (CommandsQueue.ContainsKey(request.DeviceId))
            {
                if (CommandsQueue[request.DeviceId].Count > 0)
                {
                    List<DeviceCommandResponse> queuedCommands = new();

                    // Dequeue all queued commands
                    while (CommandsQueue[request.DeviceId].TryDequeue(out var item))
                    {
                        queuedCommands.Add(item);
                    }

                    // Complete the TaskCompletionSource with the queued commands
                    tcs.TrySetResult(queuedCommands);
                }
                
            }
            return tcs;
        }

        /// <summary>
        /// Removes a pending request for a device.
        /// </summary>
        /// <param name="request">The device's poll request information.</param>
        /// <returns>True if the request was successfully removed; otherwise, false.</returns>
        public static bool RemoveRequest(DevicePollRequest request)
        {
            return PendingRequests.TryRemove(request.DeviceId, out _);
          
        }

        /// <summary>
        /// Sends a command to a device.
        /// If the device is currently polling (waiting for commands), the command is sent immediately.
        /// Otherwise, the command is queued until the device polls again.
        /// </summary>
        /// <param name="request">The command to be sent to the device.</param>
        /// <returns>True if the command was handled successfully; otherwise, false.</returns>
        public static bool SendCommand(DeviceCommandRequest request)
        {
            try
            {
                // Format the command response
                DeviceCommandResponse formattedResponse = new DeviceCommandResponse
                {
                    HasCommand = true,
                    Command = request.Command,
                    IssueDate = DateTime.UtcNow,
                    Expirationtime = 60 // 1 minute expiration time
                };

                // Check if the device is currently waiting for a command
                if (PendingRequests.TryGetValue(request.DeviceId, out var tcs))
                {
                    // Immediately send the command to the waiting device
                    tcs.TrySetResult(new List<DeviceCommandResponse>                    
                    {
                        formattedResponse
                    });

                    return true;
                }
                else // Device is not polling currently, queue the command
                {
                    if (!CommandsQueue.ContainsKey(request.DeviceId))
                    {
                        CommandsQueue.TryAdd(request.DeviceId, new ConcurrentQueue<DeviceCommandResponse>());

                    }

                    // Enqueue the command for later delivery
                    CommandsQueue[request.DeviceId].Enqueue(formattedResponse);

                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
