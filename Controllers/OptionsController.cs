using Microsoft.AspNetCore.Mvc;
using Wattmate_Site.Controllers.ViewModels;

namespace Wattmate_Site.Controllers
{
    public class OptionsController : Controller
    {
        [HttpGet]
        public IActionResult Index(string deviceId)
        {
            OptionsViewModel model = new OptionsViewModel();
            return View(model);
        }
    }
}
