using FluentValidation;

namespace LetMeKnowApi.ViewModels.Validations
{
    public class UpdateSuggestionViewModelValidator : AbstractValidator<UpdateSuggestionViewModel>
    {
        public UpdateSuggestionViewModelValidator()
        {
            RuleFor(suggestion => suggestion.Title).NotEmpty().WithMessage("Título no puede estar vacío"); 
            RuleFor(suggestion => suggestion.Title).Length(3, 60).WithMessage("Título debe tener entre 3 y 60 caractered");           

            RuleFor(suggestion => suggestion.Content).NotEmpty().WithMessage("Contenido no puede estar vacío");            
            RuleFor(suggestion => suggestion.Image).NotEmpty().WithMessage("Imagen no puede estar vacío"); 
            //RuleFor(suggestion => suggestion.Image).Matches(@"/^(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$/").WithMessage("Debe introducir una url de imagen válida");
                       
            RuleFor(suggestion => suggestion.AreaId).NotEmpty().WithMessage("Area no puede estar vacío");                                   
            
            //RuleFor(suggestion => suggestion.Status).NotEmpty().WithMessage("Status no puede estar vacío");                 
        }
    }
}
