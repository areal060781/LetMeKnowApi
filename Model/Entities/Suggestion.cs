using System;

namespace LetMeKnowApi.Model
{
    public class Suggestion : IEntityBase
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Image { get; set; }
        public SuggestionStatus Status { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }        
        public int AreaId { get;set; }                        
        public int CreatorId { get; set; }

        //Navigation Properties
        public Area Area { get; set; }
        public User Creator { get; set; }

    }
}
