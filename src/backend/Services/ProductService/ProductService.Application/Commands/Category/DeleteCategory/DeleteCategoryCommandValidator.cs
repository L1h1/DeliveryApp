using FluentValidation;

namespace ProductService.Application.Commands.Category.DeleteCategory
{
    public class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
    {
        public DeleteCategoryCommandValidator()
        {
            RuleFor(c => c.Id)
                .NotEmpty().WithMessage("Category id is empty");
        }
    }
}
