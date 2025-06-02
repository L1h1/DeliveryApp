using FluentValidation;
using OrderService.Application.DTOs.Request;

namespace OrderService.Application.Validators
{
    public class OrderRequestValidator : AbstractValidator<OrderRequestDTO>
    {
        public OrderRequestValidator()
        {
            RuleFor(x => x.ClientId)
                .NotEmpty().WithMessage("Client id is empty.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is empty.");

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Order item list is emtpty.");
        }
    }
}
