using FluentValidation;
using UserService.BLL.DTOs.Request;
using UserService.BLL.Extensions;

namespace UserService.BLL.Validators
{
    public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequestDTO>
    {
        public ResetPasswordRequestValidator()
        {
            RuleFor(d => d.ResetCode)
                .NotEmpty().WithMessage("Reset token is empty.");

            RuleFor(d => d.Email)
                .NotEmpty().WithMessage("Email is empty.")
                .EmailAddress().WithMessage("Incorrect email format.");

            RuleFor(d => d.NewPassword)
                .ApplyPasswordRules();
        }
    }
}
