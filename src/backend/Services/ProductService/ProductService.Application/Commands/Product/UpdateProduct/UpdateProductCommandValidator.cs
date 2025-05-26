using FluentValidation;

namespace ProductService.Application.Commands.Product.UpdateProduct
{
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Product id is empty.");
        }
    }
}
