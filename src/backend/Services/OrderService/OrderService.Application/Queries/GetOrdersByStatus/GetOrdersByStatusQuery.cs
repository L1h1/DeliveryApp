using MediatR;
using OrderService.Application.DTOs.Response;
using OrderService.Domain.Enums;

namespace OrderService.Application.Queries.GetOrdersByStatus
{
    public sealed record GetOrdersByStatusQuery(OrderStatus OrderStatus) : IRequest<List<OrderResponseDTO>>
    {
    }
}
