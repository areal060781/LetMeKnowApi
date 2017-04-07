using System.Collections.Generic;

namespace LetMeKnowApi.Model
{
    public class User : IEntityBase
    {
        public User()
        {
            SuggestionsCreated = new List<Suggestion>();
            Roles = new List<UserRole>();
            //Areas = new List<Area>();
        }

        public int Id { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
        public string Email { get; set; }
        //public DateTime DateCreated { get; set; }

        //Navigation Properties
        public ICollection<Suggestion> SuggestionsCreated { get; set; }        
        public ICollection<UserRole> Roles { get; set; }
        //public ICollection<Area> Areas { get; set; }
    }
}
