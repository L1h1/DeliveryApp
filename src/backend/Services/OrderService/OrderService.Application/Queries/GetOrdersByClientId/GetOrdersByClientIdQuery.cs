using MediatR;
using OrderService.Application.DTOs.Response;

namespace OrderService.Application.Queries.GetOrdersByClientId
{
    public sealed record GetOrdersByClientIdQuery(Guid Id) : IRequest<List<OrderResponseDTO>>
    {
    }
}
