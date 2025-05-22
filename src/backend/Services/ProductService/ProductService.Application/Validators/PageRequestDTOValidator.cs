using FluentValidation;
using ProductService.Application.DTOs.Request;

namespace ProductService.Application.Validators
{
    public class PageRequestDTOValidator : AbstractValidator<PageRequestDTO>
    {
        public PageRequestDTOValidator()
        {
            RuleFor(q => q.PageNumber)
                .NotEmpty().WithMessage("Invalid current page.")
                .GreaterThan(0).WithMessage("Current page can't be less than 1.");

            RuleFor(q => q.PageNumber)
                .NotEmpty().WithMessage("Invalid page size.")
                .GreaterThan(0).WithMessage("Page size can't be less than 1.");
        }
    }
}
