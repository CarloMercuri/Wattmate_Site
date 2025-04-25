namespace Wattmate_Site.UserAuthentication.Models
{
    public class UserAuthenticationRequestResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public AuthenticatedUserData? AuthenticatedUserData { get; set; }
    }
}
