using FluentValidation;

namespace ProductService.Application.Queries.Product.GetProductById
{
    public class GetProductByIdQueryValidator : AbstractValidator<GetProductByIdQuery>
    {
        public GetProductByIdQueryValidator()
        {
            RuleFor(q => q.Id)
                .NotEmpty().WithMessage("Product id is empty.");
        }
    }
}
