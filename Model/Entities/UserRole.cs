using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//namespace LetMeKnowApi.Model.Entities
namespace LetMeKnowApi.Model
{
    public class UserRole : IEntityBase
    {
        public int Id { get; set; }        
        public int RoleId { get; set; }        
        public int UserId { get; set; }

        //Navigation Properties
        public Role Role { get; set;}
        public User User { get; set; }
    }
}
