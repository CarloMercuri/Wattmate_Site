
namespace Wattmate_Site.UserAuthentication.Models
{
    public class AuthenticatedUserData
    {
        public string UserEmail { get; set; } = string.Empty;
        public string UserSamName { get; set; } = string.Empty;
        public bool IsAdmin { get; set; } = false;
        public bool CanRead { get; set; } = false;  
        public bool CanWrite { get; set; } = false; 
        public AuthenticatedUserRights UserRights { get; set; } = AuthenticatedUserRights.None;
    }
}
