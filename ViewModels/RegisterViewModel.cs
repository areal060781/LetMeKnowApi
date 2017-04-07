using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using LetMeKnowApi.ViewModels.Validations;

namespace LetMeKnowApi.ViewModels 
{
    public class RegisterViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido")]         
        public string UserName { get; set; }             

        [Required(ErrorMessage = "El correo electrónico es requerido")]
        [EmailAddress(ErrorMessage = "El correo no es una dirección de email válida")]        
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} y como máximo {1} caracteres de longitud.", MinimumLength = 6)]
        [Display(Name = "Contraseña")]   
        public string Password { get; set; }
        
        [Compare("Password", ErrorMessage = "La contraseña y la confirmación de la contraseña no coinciden.")]
        public string ConfirmPassword { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new RegisterViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
}
