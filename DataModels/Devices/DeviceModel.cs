using Wattmate_Site.DataModels.Attributes;

namespace Wattmate_Site.DataModels.Devices
{
    public class DeviceModel
    {
        /// <summary>
        /// The global ID of the device
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// The Name of the device (editable by the user)
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// The internal ID of the user this device is attached to
        /// </summary>
        [HideFromDTO]
        public string UserId { get; set; }

        /// <summary>
        /// Has the device been seen (api activity) recently
        /// </summary>
        public bool Online { get; set; }

        /// <summary>
        /// Type (FRIDGE_1 for example)
        /// </summary>
        public string DeviceType { get; set; }


        /// <summary>
        /// IS the relay active or not
        /// </summary>
        public string Status { get; set; }

        [HideFromDTO]
        public FridgeDeviceData FridgeDevice { get; set; }
    }
}
