using MediatR;
using OrderService.Application.DTOs.Response;

namespace OrderService.Application.Queries.GetOrdersByCourierId
{
    public sealed record GetOrdersByCourierIdQuery(Guid Id) : IRequest<List<OrderResponseDTO>>
    {
    }
}
