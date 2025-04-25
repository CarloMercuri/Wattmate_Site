using Microsoft.AspNetCore.Identity;

namespace Wattmate_Site.UserAuthentication.Models
{
    public class UserLoginData
    {
        public string UserEmail { get; set; }
        public string UserPassword { get; set;}

        public string Name { get; set; }
        public string Surname { get; set; }
        public string FullName => $"{Name} {Surname}";
        public string ErrorMessage { get; set; } = "";
    }
}
