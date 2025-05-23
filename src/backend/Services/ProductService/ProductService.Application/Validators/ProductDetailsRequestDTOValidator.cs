using FluentValidation;
using ProductService.Application.DTOs.Request;

namespace ProductService.Application.Validators
{
    public class ProductDetailsRequestDTOValidator : AbstractValidator<ProductDetailsRequestDTO>
    {
        public ProductDetailsRequestDTOValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Product id is empty.")
                .Must(x => Guid.TryParse(x, out _));

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is empty.")
                .MaximumLength(256).WithMessage("Description lengt can't be more than 256.")
                .When(x => x is not null);

            RuleFor(x => x.Images)
                .NotEmpty().WithMessage("Image list is empty.")
                .Must(x => x!.Count <= 7).WithMessage("Image list can't contain more than 7 images.")
                .When(x => x is not null);

            RuleFor(x => x.Composition)
                .NotEmpty().WithMessage("Composition list is empty.")
                .Must(x => x!.Count <= 20).WithMessage("Product composition can't have more than 20 items.")
                .When(x => x is not null);

            RuleFor(x => x.NutritionInfo)
                .NotEmpty().WithMessage("Nutrition info is empty.")
                .Must(x => x!.Proteins >= 0).WithMessage("Proteins value can't be less than 0.")
                .Must(x => x!.Calories >= 0).WithMessage("Calories value can't be less than 0.")
                .Must(x => x!.Carbs >= 0).WithMessage("Carbs value can't be less than 0.")
                .Must(x => x!.Fats >= 0).WithMessage("Fats value can't be less than 0.")
                .When(x => x is not null);
        }
    }
}
