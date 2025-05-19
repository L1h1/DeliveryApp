using FluentValidation;
using UserService.BLL.DTOs.Request;

namespace UserService.BLL.Validators
{
    public class TokenRequestValidator : AbstractValidator<TokenRequestDTO>
    {
        public TokenRequestValidator()
        {
            RuleFor(d => d.AccessToken)
                .NotEmpty().WithMessage("Access token is empty.");

            RuleFor(d => d.RefreshToken)
                .NotEmpty().WithMessage("Refresh token is empty.");
        }
    }
}
