using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using LetMeKnowApi.ViewModels.Validations;

namespace LetMeKnowApi.ViewModels
{
    public class UpdateSuggestionViewModel : IValidatableObject
    {
        public int Id { get; set; }                
        public string Title { get; set; }        
        public string Content { get; set; }
        //[DataType(DataType.ImageUrl)]
        public string Image { get; set; }
        public string Status { get; set; }        
        public DateTime DateUpdated { get; set; }        
        public string Area { get; set; }
        public int AreaId {get; set;}        

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new UpdateSuggestionViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
}
