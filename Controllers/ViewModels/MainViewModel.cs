using Wattmate_Site.DataModels;
using Wattmate_Site.DataModels.DataTransferModels;

namespace Wattmate_Site.Controllers.ViewModels
{
    public class MainViewModel
    {
        public UserModelDTO UserData { get; set; }
     
        public List<DeviceModelDTO> Devices { get; set; }
    }
}
