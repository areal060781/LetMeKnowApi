using FluentValidation;

namespace LetMeKnowApi.ViewModels.Validations
{
    public class RoleViewModelValidator : AbstractValidator<RoleViewModel>
    {
        public RoleViewModelValidator()
        {
            RuleFor(role => role.Name).NotEmpty().WithMessage("El nombre del rol no debe estar vacío"); 
            RuleFor(role => role.Name).Length(4, 40).WithMessage("El nombre del rol debe tener entre 4 y 40 caracteres");           
        }
    }
}
