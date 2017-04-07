using System.Collections.Generic;

namespace LetMeKnowApi.ViewModels
{
    public class UserDetailsViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }               
        public string Email { get; set; }
        public int SuggestionsCreated { get; set; }        
        public ICollection<RoleViewModel> Roles { get; set; }       
    }
}