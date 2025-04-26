namespace Wattmate_Site.Security.Models
{
    public class NewPasswordHashResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        
        public PasswordHashData PasswordData { get; set; }
    }
}
