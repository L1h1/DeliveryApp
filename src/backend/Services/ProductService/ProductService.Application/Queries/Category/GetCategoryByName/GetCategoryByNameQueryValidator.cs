using FluentValidation;

namespace ProductService.Application.Queries.Category.GetCategoryByName
{
    public class GetCategoryByNameQueryValidator : AbstractValidator<GetCategoryByNameQuery>
    {
        public GetCategoryByNameQueryValidator()
        {
            RuleFor(q => q.name)
                .NotEmpty().WithMessage("Category name is empty.")
                .MaximumLength(64).WithMessage("Category name's length can't be more than 64");
        }
    }
}
