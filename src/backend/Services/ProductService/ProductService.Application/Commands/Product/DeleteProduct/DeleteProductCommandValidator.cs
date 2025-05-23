using FluentValidation;

namespace ProductService.Application.Commands.Product.DeleteProduct
{
    public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
    {
        public DeleteProductCommandValidator()
        {
            RuleFor(x => x.id)
                .NotEmpty().WithMessage("Product id is empty.")
                .Must(x => Guid.TryParse(x, out _));
        }
    }
}
