using MediatR;
using ProductService.Application.DTOs.Request;
using ProductService.Application.DTOs.Response;

namespace ProductService.Application.Queries.Product.GetFilteredProducts
{
    public sealed record GetFilteredProductsQuery(FilterProductsRequestDTO Dto) : IRequest<PaginatedResponseDTO<ProductResponseDTO>>
    {
    }
}
