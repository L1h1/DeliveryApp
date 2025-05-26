using FluentValidation;

namespace ProductService.Application.Commands.Category.UpdateCategory
{
    public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
    {
        public UpdateCategoryCommandValidator()
        {
            RuleFor(x => x.id)
                .NotEmpty().WithMessage("Category id is empty.");
        }
    }
}
