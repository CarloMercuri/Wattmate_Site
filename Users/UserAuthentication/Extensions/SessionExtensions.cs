using System.Text.Json;
using Wattmate_Site.DataModels;
using Wattmate_Site.Users.UserAuthentication.Models;

namespace Wattmate_Site.Users.UserAuthentication.Extensions
{
    public static class SessionExtensions
    {
        public static void SetUserData(this ISession session, UserModel? _userData)
        {
            if (_userData is null)
            {
                session.Remove("UserEmail");
                return;
            }
            else
            {
                session.SetString("UserEmail", JsonSerializer.Serialize(_userData));
            }
        }

        public static UserModel? GetUserData(this ISession session)
        {
            var value = session.GetString("UserEmail");
            return value == null ? null : JsonSerializer.Deserialize<UserModel>(value);
        }

        //public static bool IsUserAdmin(this ISession session)
        //{
        //    var value = session.GetString("UserEmail");
        //    if (value is null) return false;

        //    return JsonSerializer.Deserialize<UserModel>(value).IsAdmin;
        //}

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
