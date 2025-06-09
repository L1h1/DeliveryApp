using FluentValidation;

namespace OrderService.Application.Queries.GetOrdersByCourierId
{
    public class GetOrdersByCourierIdQueryValidator : AbstractValidator<GetOrdersByCourierIdQuery>
    {
        public GetOrdersByCourierIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Client id is empty.");
        }
    }
}
