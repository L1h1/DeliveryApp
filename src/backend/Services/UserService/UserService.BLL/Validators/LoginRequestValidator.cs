using FluentValidation;
using UserService.BLL.DTOs.Request;
using UserService.BLL.Extensions;

namespace UserService.BLL.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequestDTO>
    {
        public LoginRequestValidator()
        {
            RuleFor(d => d.Email)
                .NotEmpty().WithMessage("Email is empty.")
                .EmailAddress().WithMessage("Incorrect email format.");

            RuleFor(d => d.Password)
                .ApplyPasswordRules();
        }
    }
}
