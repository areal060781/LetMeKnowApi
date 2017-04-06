using FluentValidation;

namespace LetMeKnowApi.ViewModels.Validations
{
    public class AreaViewModelValidator : AbstractValidator<AreaViewModel>
    {
        public AreaViewModelValidator()
        {
            RuleFor(area => area.Name).NotEmpty().WithMessage("El nombre del Área no debe estar vacío");   
            RuleFor(area => area.Name).Length(4, 60).WithMessage("El nombre del Área debe tener entre 4 y 60 caracteres");         
        }
    }
}
