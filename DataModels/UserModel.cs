namespace Wattmate_Site.DataModels
{
    public class UserModel
    {
        public string UserEmail { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string FullName => Name + " " + Surname;
    }
}
