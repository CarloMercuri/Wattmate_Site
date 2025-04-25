namespace Wattmate_Site.Security.Models
{
    public class NewPasswordHashResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string HashedPassword { get; set; }
        public string Salt { get; set; }
        public int Iterations { get; set; }
    }
}
