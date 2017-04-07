using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using LetMeKnowApi.ViewModels.Validations;

namespace LetMeKnowApi.ViewModels
{
    public class RoleViewModel : IValidatableObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Users { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext ValidationContext)
        {
            var validator = new RoleViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
}
