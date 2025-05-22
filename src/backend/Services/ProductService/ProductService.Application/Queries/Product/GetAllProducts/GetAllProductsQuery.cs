using MediatR;
using ProductService.Application.DTOs.Request;
using ProductService.Application.DTOs.Response;

namespace ProductService.Application.Queries.Product.GetAllProducts
{
    public sealed record GetAllProductsQuery(PageRequestDTO dto) : IRequest<PaginatedResponseDTO<ProductResponseDTO>>
    {
    }
}
