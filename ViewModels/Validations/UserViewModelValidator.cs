using FluentValidation;

namespace LetMeKnowApi.ViewModels.Validations
{
    public class UserViewModelValidator : AbstractValidator<UserViewModel>
    {
        public UserViewModelValidator()
        {
            RuleFor(user => user.UserName).NotEmpty().WithMessage("El nombre de usuario no puede ir vacío");
            RuleFor(user => user.UserName).Length(4, 28).WithMessage("El nombre de usuario");
            RuleFor(user => user.UserName).Matches(@"/^[a-z_\d]{4,28}$/i").WithMessage("1 El nombre de usuario debe ser entre 4 y 28 caracteres");
            RuleFor(user => user.UserName).Matches("/^[a-z]{1}[a-z0-9_]{3,13}$/").WithMessage("2 El nombre de usuario debe ser entre 4 y 28 caracteres");
            RuleFor(user => user.UserName).Matches("/^[a-z0-9_-]{3,16}$/").WithMessage("3 El nombre de usuario debe ser entre 4 y 28 caracteres");
            RuleFor(user => user.UserName).Matches(@"\A\S{4,28}\z").WithMessage("4 El nombre de usuario debe ser entre 4 y 28 caracteres");
            

            RuleFor(user => user.PassWordHash).NotEmpty().WithMessage("Password cannot be empty");
            RuleFor(user => user.Salt).NotEmpty().WithMessage("Salt cannot be empty");

            RuleFor(user => user.Email).NotEmpty().WithMessage("El correo no puede estar vacío");            
            RuleFor(user => user.Email).EmailAddress().WithMessage("Debe introducir un correo válido");            
        }
    }
}
