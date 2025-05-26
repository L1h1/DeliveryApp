using MediatR;
using ProductService.Application.DTOs.Response;

namespace ProductService.Application.Queries.Product.GetProductById
{
    public sealed record GetProductByIdQuery(Guid id) : IRequest<DetailedProductResponseDTO>
    {
    }
}
