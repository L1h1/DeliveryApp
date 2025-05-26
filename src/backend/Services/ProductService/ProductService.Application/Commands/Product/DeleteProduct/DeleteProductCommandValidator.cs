using FluentValidation;

namespace ProductService.Application.Commands.Product.DeleteProduct
{
    public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
    {
        public DeleteProductCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Product id is empty.");
        }
    }
}
