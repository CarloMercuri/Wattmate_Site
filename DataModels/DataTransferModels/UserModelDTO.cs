using Wattmate_Site.DataModels.Attributes;

namespace Wattmate_Site.DataModels.DataTransferModels
{
    public class UserModelDTO
    {
        /// <summary>
        /// User (Login) name 
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Real name of our user
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Real surname of our user
        /// </summary>
        public string Surname { get; set; }


        /// <summary>
        /// Formats the name in <Name> <Surname> version
        /// </summary>
        public string FullName => Name + " " + Surname;
    }
}
