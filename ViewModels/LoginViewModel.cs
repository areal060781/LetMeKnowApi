using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using LetMeKnowApi.ViewModels.Validations;

namespace LetMeKnowApi.ViewModels
{
    public class LoginViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        public string UserName { get; set; }

        /*[Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }*/

        [Required(ErrorMessage = "La contraseña es requerida")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        //[Display(Name = "Remember me?")]
        //public bool RememberMe { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new LoginViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
}
