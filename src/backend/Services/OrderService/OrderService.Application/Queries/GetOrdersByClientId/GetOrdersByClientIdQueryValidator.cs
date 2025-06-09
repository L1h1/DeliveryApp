using FluentValidation;

namespace OrderService.Application.Queries.GetOrdersByClientId
{
    public class GetOrdersByClientIdQueryValidator : AbstractValidator<GetOrdersByClientIdQuery>
    {
        public GetOrdersByClientIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Client id is empty.");
        }
    }
}
