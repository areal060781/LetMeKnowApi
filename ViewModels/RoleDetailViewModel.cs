using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LetMeKnowApi.ViewModels.Validations;

namespace LetMeKnowApi.ViewModels
{
    public class RoleDetailViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }        
        public ICollection<UserViewModel> Users { get; set; }        
    }
}
