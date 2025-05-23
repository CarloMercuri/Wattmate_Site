using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Wattmate_Site.Controllers.Attributes;
using Wattmate_Site.Models;
using Wattmate_Site.UserAuthentication.Extensions;
using Wattmate_Site.UserAuthentication.Interfaces;
using Wattmate_Site.UserAuthentication.Models;

namespace Wattmate_Site.Controllers
{
    public class HomeController : SecureAccessController
    {
        IWattmateAuthenticationService _authProcessor;

        public HomeController(IWattmateAuthenticationService auth)
        {
           _authProcessor = auth;
        }

        [AuthenticationRequired]
        public IActionResult Index()
        {
            return View();
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
                HttpContext.Session.SetUserData(result.AuthenticatedUserData);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                UserLoginData _status = new UserLoginData();
                _status.ErrorMessage = result.Message;
                TempData["errorMessage"] = result.Message;
                return RedirectToAction("Login", "Home");
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

        [HttpPost]
        public IActionResult CreateNewUser([FromBody] UserLoginData data)
        {
            return Ok();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
