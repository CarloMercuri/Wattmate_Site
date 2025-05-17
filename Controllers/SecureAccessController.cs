using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;
using Wattmate_Site.Controllers.Attributes;
using Wattmate_Site.Controllers.DeviceController;
using Wattmate_Site.DataModels;
using Wattmate_Site.Security.DeviceAuthentication;
using Wattmate_Site.Users.UserAuthentication.Extensions;
using Wattmate_Site.Users.UserAuthentication.Models;

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
                // Check if the HMAC is valid
                if (!AuthenticateDevice(context))
                {
                    return;
                }
            }
            base.OnActionExecuting(context);
        }

        private bool AuthenticateDevice(ActionExecutingContext context)
        {
            // Try to get the DevicePollRequest from action arguments
            if (context.ActionArguments.TryGetValue("request", out var requestObj) &&
                requestObj is DevicePollRequest request)
            {
                var hmacValid = DeviceAuthenticationProcessor.IsDeviceGenuine(request);
  
                if (!hmacValid)
                {
                    context.Result = new UnauthorizedResult();
                    return false;
                }
            }
            else
            {
                context.Result = new BadRequestObjectResult("Invalid or missing request body.");
                return false;
            }

            // If valid, process the request
            return true;
        }
    }
}
