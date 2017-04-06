using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LetMeKnowApi.ViewModels.Validations;

namespace LetMeKnowApi.ViewModels
{
    public class AreaViewModel : IValidatableObject
    {
        public int Id { get; set; }        
        public string Name { get; set; }
        public int Suggestions { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext ValidationContext)
        {
            var validator = new AreaViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
}
