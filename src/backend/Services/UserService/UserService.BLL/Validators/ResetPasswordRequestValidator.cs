using FluentValidation;
using UserService.BLL.DTOs.Request;

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
                .NotEmpty().WithMessage("Password is empty")
                .MinimumLength(6).WithMessage("Password requires minimum length of 6.")
                .Matches(@"[A-Z]").WithMessage("Password requires an upper case letter.")
                .Matches(@"[a-z]").WithMessage("Password requires a lower case letter.")
                .Matches(@"\d").WithMessage("Password requires a numeric digit.")
                .Matches(@"[\W]").WithMessage("Password requires a special character.");
        }
    }
}
