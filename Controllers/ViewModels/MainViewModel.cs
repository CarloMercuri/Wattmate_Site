using Wattmate_Site.DataModels;

namespace Wattmate_Site.Controllers.ViewModels
{
    public class MainViewModel
    {
        public UserModel UserData { get; set; }
     
        public List<DeviceModel> Devices { get; set; }
    }
}
