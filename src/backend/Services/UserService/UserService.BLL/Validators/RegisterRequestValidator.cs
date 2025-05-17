using FluentValidation;
using UserService.BLL.DTOs.Request;

namespace UserService.BLL.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequestDTO>
    {
        public RegisterRequestValidator()
        {
            RuleFor(d => d.UserName)
                .NotEmpty().WithMessage("Username is empty.")
                .MaximumLength(16).WithMessage("Username maximum length exceeded (16).");

            RuleFor(d => d.Email)
                .NotEmpty().WithMessage("Email is empty.")
                .EmailAddress().WithMessage("Incorrect email format.");

            RuleFor(d => d.Password)
                .NotEmpty().WithMessage("Password is empty")
                .MinimumLength(6).WithMessage("Password requires minimum length of 6.")
                .Matches(@"[A-Z]").WithMessage("Password requires an upper case letter.")
                .Matches(@"[a-z]").WithMessage("Password requires a lower case letter.")
                .Matches(@"\d").WithMessage("Password requires a numeric digit.")
                .Matches(@"[\W]").WithMessage("Password requires a special character.");

            RuleFor(d => d.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is empty.")
                .Matches(@"^\+\d[\d\s\-\(\)]{6,18}\d$").WithMessage("Incorrect phone number format.");
        }
    }
}
