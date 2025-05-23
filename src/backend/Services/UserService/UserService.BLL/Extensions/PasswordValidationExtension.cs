using FluentValidation;

namespace UserService.BLL.Extensions
{
    public static class PasswordValidationExtension
    {
        public static IRuleBuilderOptions<T, string> ApplyPasswordRules<T>(this IRuleBuilder<T, string> rule)
        {
            return rule
                .NotEmpty().WithMessage("Password is empty")
                .MinimumLength(6).WithMessage("Password requires minimum length of 6.")
                .Matches(@"[A-Z]").WithMessage("Password requires an upper case letter.")
                .Matches(@"[a-z]").WithMessage("Password requires a lower case letter.")
                .Matches(@"\d").WithMessage("Password requires a numeric digit.")
                .Matches(@"[\W]").WithMessage("Password requires a special character.");
        }
    }
}
