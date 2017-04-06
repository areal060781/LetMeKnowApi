using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//namespace LetMeKnowApi.Model.Entities
namespace LetMeKnowApi.Model
{
    public class Area : IEntityBase
    {
        public Area()
        {
            Suggestions = new List<Suggestion>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        //public DateTime DateCreated { get; set; }        
        //public int UserId { get; set; }

        //Navigation Properties        
        public ICollection<Suggestion> Suggestions { get; set; }
        //public User User { get; set; }
    }
}
