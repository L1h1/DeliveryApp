using FluentValidation;

namespace OrderService.Application.Commands.UpdateOrderStatus
{
    public class UpdateOrderStatusCommandValidator : AbstractValidator<UpdateOrderStatusCommand>
    {
        public UpdateOrderStatusCommandValidator()
        {
            RuleFor(x => x.OrderId)
                .NotEmpty().WithMessage("Order id is empty.");

            RuleFor(x => x.OrderStatus)
                .NotEmpty().WithMessage("Order status is empty.")
                .IsInEnum().WithMessage("Incorrect order status.");
        }
    }
}
