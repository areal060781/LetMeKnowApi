using FluentValidation;

namespace LetMeKnowApi.ViewModels.Validations
{
    public class LoginViewModelValidator : AbstractValidator<LoginViewModel>
    {
        public LoginViewModelValidator()
        {            
            RuleFor(user => user.UserName).Matches(@"\A\S{4,28}\z").WithMessage("4 El nombre de usuario debe tener entre 4 y 28 caracteres de longitud y no puede contener espacios");                          
        }
    }
}
