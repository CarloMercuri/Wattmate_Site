using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;
using Wattmate_Site.Controllers.Attributes;
using Wattmate_Site.UserAuthentication.Extensions;
using Wattmate_Site.UserAuthentication.Models;

namespace Wattmate_Site.Controllers
{
    public class SecureAccessController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Type controllerType = ((ControllerBase)context.Controller).ControllerContext.ActionDescriptor.ControllerTypeInfo.AsType();
            var actionName = ((ControllerBase)context.Controller).ControllerContext.ActionDescriptor.ActionName;
            var action = controllerType.GetMethod(actionName);

            AuthenticatedUserData? _authenticatedUser = HttpContext.Session.GetUserData();

            AuthenticationRequiredAttribute _authRequired = (AuthenticationRequiredAttribute)action.GetCustomAttribute(typeof(AuthenticationRequiredAttribute), true);

            if (_authRequired != null)
            {
                if (_authenticatedUser == null)
                {
                    context.Result = RedirectToAction("Login", "Home");

                }
            }

            // Check the right permission

            //PermissionRequiredAttribute _permissionRequired = (PermissionRequiredAttribute)action.GetCustomAttribute(typeof(PermissionRequiredAttribute), true);

            //if (_permissionRequired != null)
            //{
            //    if (_authenticatedUser == null)
            //    {
            //        context.Result = RedirectToAction("Login", "Home");

            //    }
            //    else if (_permissionRequired.RequiredRights > _authenticatedUser.UserRights)
            //    {
            //        context.Result = Json(new InsufficentRightsReturnModel() { Access = false, Message = "Insufficent rights to perform this action" });
            //    }
            //}
            base.OnActionExecuting(context);
        }
    }
}
