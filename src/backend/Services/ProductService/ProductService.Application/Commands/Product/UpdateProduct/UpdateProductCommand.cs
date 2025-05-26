using MediatR;
using ProductService.Application.DTOs.Request;
using ProductService.Application.DTOs.Response;

namespace ProductService.Application.Commands.Product.UpdateProduct
{
    public sealed record UpdateProductCommand(Guid Id, ProductRequestDTO RequestDTO) : IRequest<ProductResponseDTO>
    {
    }
}
