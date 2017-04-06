using LetMeKnowApi.ViewModels.Validations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace LetMeKnowApi.ViewModels
{
    public class UserViewModel : IValidatableObject
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PassWordHash { get; set; }
        public string Salt { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public int SuggestionsCreated { get; set; }
        public int Roles { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new UserViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
}
