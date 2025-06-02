using FluentValidation;

namespace OrderService.Application.Commands.AssignCourier
{
    public class AssignCourierCommandValidator : AbstractValidator<AssignCourierCommand>
    {
        public AssignCourierCommandValidator()
        {
            RuleFor(x => x.OrderId)
                .NotEmpty().WithMessage("Order id is empty.");

            RuleFor(x => x.CourierId)
                .NotEmpty().WithMessage("Courier id is empty.");
        }
    }
}
