using FluentValidation;
using ProductService.Application.DTOs.Request;

namespace ProductService.Application.Validators
{
    public class CategoryRequestDTOValidator : AbstractValidator<CategoryRequestDTO>
    {
        public CategoryRequestDTOValidator()
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Category name is empty.")
                .MaximumLength(64).WithMessage("Category name can't be longther than 64 characters.");
        }
    }
}
