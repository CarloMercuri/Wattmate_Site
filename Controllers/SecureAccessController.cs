using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;
using Wattmate_Site.Controllers.Attributes;
using Wattmate_Site.Controllers.DeviceController;
using Wattmate_Site.DataModels;
using Wattmate_Site.Security.DeviceAuthentication;
using Wattmate_Site.Users.UserAuthentication.Extensions;
using Wattmate_Site.Users.UserAuthentication.Models;
using Wattmate_Site.WLog;

namespace Wattmate_Site.Controllers
{
    public class SecureAccessController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Type controllerType = ((ControllerBase)context.Controller).ControllerContext.ActionDescriptor.ControllerTypeInfo.AsType();
            var actionName = ((ControllerBase)context.Controller).ControllerContext.ActionDescriptor.ActionName;
            var action = controllerType.GetMethod(actionName);

            UserModel? _authenticatedUser = HttpContext.Session.GetUserData();

            AuthenticationRequiredAttribute _authRequired = (AuthenticationRequiredAttribute)action.GetCustomAttribute(typeof(AuthenticationRequiredAttribute), true);

            if (_authRequired != null)
            {
                if (_authenticatedUser == null)
                {
                    context.Result = RedirectToAction("Login", "Home");

                }
            }

            // Check if the endpoint needs to have a device HMAC authentication

            DeviceHmacAuthenticationRequiredAttribute _hmacAuthRequired = 
                (DeviceHmacAuthenticationRequiredAttribute)action.GetCustomAttribute(typeof(DeviceHmacAuthenticationRequiredAttribute), true);

            if (_hmacAuthRequired != null)
            {
                //Check if the HMAC is valid
                //if (!AuthenticateDevice(context))
                //{
                //    context.Result = new UnauthorizedResult();
                //    return;
                //}
            }
            base.OnActionExecuting(context);
        }

        private bool AuthenticateDevice(ActionExecutingContext context)
        {
            string? hmacHeader = context.HttpContext.Request.Headers["X-Device-Hmac"].FirstOrDefault();
            string? timeStampHeader = context.HttpContext.Request.Headers["X-Device-Timestamp"].FirstOrDefault();
            string? idHeader = context.HttpContext.Request.Headers["X-Device-DeviceId"].FirstOrDefault();
       

            var hmacValid = DeviceAuthenticationProcessor.IsDeviceGenuine(idHeader, timeStampHeader, hmacHeader);

            if (!hmacValid)
            {
                return false;
            }

            // If valid, process the request
            return true;
        }
    }
}
