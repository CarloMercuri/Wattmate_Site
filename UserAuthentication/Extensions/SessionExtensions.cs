using System.Text.Json;
using Wattmate_Site.UserAuthentication.Models;

namespace Wattmate_Site.UserAuthentication.Extensions
{
    public static class SessionExtensions
    {
        public static void SetUserData(this ISession session, AuthenticatedUserData? _userData)
        {
            if(_userData is null)
            {
                session.Remove("UserEmail");
                return;
            }
            else
            {
                session.SetString("UserEmail", JsonSerializer.Serialize(_userData));
            }
        }

        public static AuthenticatedUserData? GetUserData(this ISession session)
        {
            var value = session.GetString("UserEmail");
            return value == null ? null : JsonSerializer.Deserialize<AuthenticatedUserData>(value);
        }

        public static bool IsUserAdmin(this ISession session)
        {
            var value = session.GetString("UserEmail");
            if (value is null) return false;

            return JsonSerializer.Deserialize<AuthenticatedUserData>(value).IsAdmin;
        }

        public static bool IsUserLoggedIn(this ISession session)
        {
            var value = session.GetString("UserEmail");
            if (value is null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    }
}
