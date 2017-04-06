using LetMeKnowApi.ViewModels.Validations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace LetMeKnowApi.ViewModels
{
    public class UserDetailViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PassWordHash { get; set; }
        public string Salt { get; set; }        
        public string Email { get; set; }
        public int SuggestionsCreated { get; set; }        
        public ICollection<RoleViewModel> Roles { get; set; }       
    }
}
