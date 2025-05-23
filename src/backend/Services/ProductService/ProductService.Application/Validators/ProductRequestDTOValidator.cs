using FluentValidation;
using ProductService.Application.DTOs.Request;

namespace ProductService.Application.Validators
{
    public class ProductRequestDTOValidator : AbstractValidator<ProductRequestDTO>
    {
        public ProductRequestDTOValidator()
        {
            RuleFor(p => p.Title)
                .NotEmpty().WithMessage("Product title is empty.")
                .MaximumLength(64).WithMessage("Product title can't be longer than 64.");

            RuleFor(p => p.Price)
                .NotEmpty().WithMessage("Product price is empty.")
                .GreaterThan(0).WithMessage("Product price can't be less than or equal 0.");

            RuleFor(p => p.IsAvailable)
                .NotEmpty().WithMessage("Product isAvailable flag is empty.");

            RuleFor(p => p.UnitOfMeasure)
                .NotEmpty().WithMessage("Product unitOfMeasure is empty.");
        }
    }
}
