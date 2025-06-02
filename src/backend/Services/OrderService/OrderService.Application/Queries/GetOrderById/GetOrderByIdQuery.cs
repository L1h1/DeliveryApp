using MediatR;
using OrderService.Application.DTOs.Response;

namespace OrderService.Application.Queries.GetOrderById
{
    public sealed record GetOrderByIdQuery(Guid OrderId) : IRequest<DetailedOrderResponseDTO>
    {
    }
}
