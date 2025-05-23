using FluentValidation;

namespace ProductService.Application.Commands.Category.DeleteCategory
{
    public class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
    {
        public DeleteCategoryCommandValidator()
        {
            RuleFor(c => c.id)
                .NotEmpty().WithMessage("Category id is empty")
                .Must(c => Guid.TryParse(c, out _));
        }
    }
}
