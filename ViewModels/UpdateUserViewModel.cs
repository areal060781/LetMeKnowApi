using System.ComponentModel.DataAnnotations;

namespace LetMeKnowApi.ViewModels 
{
    public class UpdateUserViewModel// : IValidatableObject
    {                  
        [Required(ErrorMessage = "El correo electrónico es requerido")]
        [EmailAddress(ErrorMessage = "El correo no es una dirección de email válida")]        
        public string Email { get; set; }
        
        /*public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new UpdateUserViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }*/
    }
}
