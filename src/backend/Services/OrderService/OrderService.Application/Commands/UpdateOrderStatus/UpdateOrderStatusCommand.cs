using MediatR;
using OrderService.Domain.Enums;

namespace OrderService.Application.Commands.UpdateOrderStatus
{
    public sealed record UpdateOrderStatusCommand(Guid OrderId, OrderStatus OrderStatus) : IRequest<Unit>
    {
    }
}
