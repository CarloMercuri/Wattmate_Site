namespace Wattmate_Site.Security.Models
{
    public class PasswordHashData
    {
        public string HashedPassword { get; set; }
        public string Salt { get; set; }
        public int Iterations { get; set; }
        public string Algorithm { get; set; }
    }
}
