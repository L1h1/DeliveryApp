using FluentValidation;

namespace ProductService.Application.Commands.ProductDetails.DeleteProductDetails
{
    public class DeleteProductDetailsCommandValidator : AbstractValidator<DeleteProductDetailsCommand>
    {
        public DeleteProductDetailsCommandValidator()
        {
            RuleFor(x => x.id)
                .NotEmpty().WithMessage("Product id is empty.");
        }
    }
}
