namespace Wattmate_Site.Users.UserAuthentication.Models
{
    public class UserAuthenticationRequestResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public AuthenticatedUserData? AuthenticatedUserData { get; set; }
    }
}
