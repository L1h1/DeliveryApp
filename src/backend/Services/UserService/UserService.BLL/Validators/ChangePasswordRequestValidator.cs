using FluentValidation;
using UserService.BLL.DTOs.Request;
using UserService.BLL.Extensions;

namespace UserService.BLL.Validators
{
    public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordDTO>
    {
        public ChangePasswordRequestValidator()
        {
            RuleFor(d => d.CurrentPassword)
                .ApplyPasswordRules();

            RuleFor(d => d.NewPassword)
                .ApplyPasswordRules()
                .NotEqual(d => d.CurrentPassword).WithMessage("New password must be different.");
        }
    }
}
