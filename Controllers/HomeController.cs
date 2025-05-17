using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Wattmate_Site.Controllers.Attributes;
using Wattmate_Site.Controllers.ViewModels;
using Wattmate_Site.DataModels;
using Wattmate_Site.DataModels.DataTransferModels;
using Wattmate_Site.DataModels.Translators;
using Wattmate_Site.Devices;
using Wattmate_Site.Models;
using Wattmate_Site.Users.UserAuthentication.Extensions;
using Wattmate_Site.Users.UserAuthentication.Interfaces;
using Wattmate_Site.Users.UserAuthentication.Models;
using Wattmate_Site.WDatabase.Interfaces;

namespace Wattmate_Site.Controllers
{
    public class HomeController : SecureAccessController
    {
        IWattmateAuthenticationService _authProcessor;
        DeviceProcessor _deviceProcessor;

        public HomeController(IWattmateAuthenticationService auth, IWDatabaseQueries db)
        {
            _deviceProcessor = new DeviceProcessor(db);
           _authProcessor = auth;
        }

        [AuthenticationRequired]
        public IActionResult Index()
        {
            UserModel _authenticatedUser = HttpContext.Session.GetUserData();

            MainViewModel model = new MainViewModel();
            model.UserData = ModelsTranslator.ToDTO<UserModel, UserModelDTO>(_authenticatedUser);

            List<DeviceModel> devices = _deviceProcessor.GetUserDevices(_authenticatedUser.UserName);
            model.Devices = ModelsTranslator.ListToDTO<DeviceModel, DeviceModelDTO>(devices);

            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.SetUserData(null);
            return RedirectToAction("Login", "Home");
        }

        [ValidateAntiForgeryToken]
        public IActionResult LoginPost(UserLoginData _userData)
        {
            UserAuthenticationRequestResult result = _authProcessor.AuthenticateUser(_userData.UserEmail, _userData.UserPassword);
            if (result.Success)
            {
                HttpContext.Session.SetUserData(result.UserData);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                UserLoginData _status = new UserLoginData();
                _status.ErrorMessage = result.Message;
                TempData["errorMessage"] = result.Message;
                return RedirectToAction("Login", "Home", result);
            }
        }

        [LocalModeOnly]
        [HttpPost]
        public IActionResult LoginApi([FromBody]UserLoginData _userData)
        {
            UserAuthenticationRequestResult result = _authProcessor.AuthenticateUser(_userData.UserEmail, _userData.UserPassword);
            if (result.Success)
            {
                return Json(result.UserData);
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            UserLoginData _status = new UserLoginData();
            string error = TempData["errorMessage"] as string;
            _status.ErrorMessage = error is null ? "" : error;
            return View(_status);
        }


        [LocalModeOnly]
        [HttpPost]
        public IActionResult CreateNewUser([FromBody] UserLoginData data)
        {
            try
            {
                UserCreationRequestResult result = _authProcessor.CreateNewUser(data);
                if (result.Success)
                {
                    return Ok("User created.");
                }
                else
                {
                    return BadRequest(result.Message);
                }
            }
            catch (Exception ex)
            {
                return Error();
            }
          
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
