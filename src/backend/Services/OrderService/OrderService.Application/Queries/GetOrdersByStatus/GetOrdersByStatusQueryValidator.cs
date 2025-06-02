using FluentValidation;

namespace OrderService.Application.Queries.GetOrdersByStatus
{
    public class GetOrdersByStatusQueryValidator : AbstractValidator<GetOrdersByStatusQuery>
    {
        public GetOrdersByStatusQueryValidator()
        {
            RuleFor(x => x.OrderStatus)
                .NotEmpty().WithMessage("Order staus is empty")
                .IsInEnum().WithMessage("Invalid order status provided.");
        }
    }
}
