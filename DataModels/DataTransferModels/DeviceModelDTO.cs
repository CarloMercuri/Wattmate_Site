namespace Wattmate_Site.DataModels.DataTransferModels
{
    public class DeviceModelDTO
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
        /// Has the device been seen (api activity) recently
        /// </summary>
        public bool Online { get; set; }

        /// <summary>
        /// IS the relay active or not
        /// </summary>
        public string Status { get; set; }
    }
}
