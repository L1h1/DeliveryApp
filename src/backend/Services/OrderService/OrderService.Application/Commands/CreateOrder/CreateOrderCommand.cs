using MediatR;
using OrderService.Application.DTOs.Request;
using OrderService.Application.DTOs.Response;

namespace OrderService.Application.Commands.CreateOrder
{
    public sealed record CreateOrderCommand(OrderRequestDTO RequestDTO) : IRequest<OrderResponseDTO>
    {
    }
}
