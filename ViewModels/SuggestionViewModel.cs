using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LetMeKnowApi.ViewModels.Validations;

namespace LetMeKnowApi.ViewModels
{
    public class SuggestionViewModel : IValidatableObject
    {
        public int Id { get; set; }
        
        //[Required]
        //[StringLength(60, MinimumLength = 3)]
        public string Title { get; set; }
        //[Required]
        public string Content { get; set; }
        //[DataType(DataType.ImageUrl)]
        public string Image { get; set; }
        public string Status { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public string Creator { get; set; }
        public int CreatorId { get; set; }
        public string Area { get; set; }
        public int AreaId {get; set;}        

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new SuggestionViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
}
