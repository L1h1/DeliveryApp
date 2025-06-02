using FluentValidation;

namespace OrderService.Application.Queries.GetOrderById
{
    public class GetOrderByIdQueryValidator : AbstractValidator<GetOrderByIdQuery>
    {
        public GetOrderByIdQueryValidator()
        {
            RuleFor(o => o.OrderId)
                .NotEmpty().WithMessage("Order id is empty.");
        }
    }
}
