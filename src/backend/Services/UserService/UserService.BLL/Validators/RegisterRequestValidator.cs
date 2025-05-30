using FluentValidation;
using UserService.BLL.DTOs.Request;
using UserService.BLL.Extensions;

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
                .ApplyPasswordRules();

            RuleFor(d => d.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is empty.")
                .Matches(@"^\+\d[\d\s\-\(\)]{6,18}\d$").WithMessage("Incorrect phone number format.");
        }
    }
}
