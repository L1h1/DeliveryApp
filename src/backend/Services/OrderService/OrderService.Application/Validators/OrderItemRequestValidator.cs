using FluentValidation;
using OrderService.Application.DTOs.Request;

namespace OrderService.Application.Validators
{
    public class OrderItemRequestValidator : AbstractValidator<OrderItemRequestDTO>
    {
        public OrderItemRequestValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Product id is empty.");
            RuleFor(x => x.Quantity)
                .NotEmpty().WithMessage("Product quantity is empty.")
                .GreaterThan(0).WithMessage("Invalid product quantity. Must be greater than 0.");
        }
    }
}
