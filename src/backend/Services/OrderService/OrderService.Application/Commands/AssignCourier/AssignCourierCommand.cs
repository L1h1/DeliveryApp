using MediatR;

namespace OrderService.Application.Commands.AssignCourier
{
    public sealed record AssignCourierCommand(Guid OrderId, Guid CourierId) : IRequest<Unit>
    {
    }
}
