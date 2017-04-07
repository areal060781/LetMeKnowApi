using System.Collections.Generic;

namespace LetMeKnowApi.ViewModels
{
    public class RoleDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }        
        public ICollection<UserViewModel> Users { get; set; }        
    }
}