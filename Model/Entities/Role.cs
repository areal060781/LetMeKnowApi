using System.Collections.Generic;

namespace LetMeKnowApi.Model
{
    public class Role : IEntityBase
    {
        public Role()
        {
            Users = new List<UserRole>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        //public DateTime DateCreated { get; set; }  
        
        //Navigation Properties        
        public ICollection<UserRole> Users { get; set; }
    }
}
