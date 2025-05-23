using MediatR;
using ProductService.Application.DTOs.Request;
using ProductService.Application.DTOs.Response;

namespace ProductService.Application.Commands.Product.UpdateProduct
{
    public sealed record UpdateProductCommand(string id, ProductRequestDTO requestDTO) : IRequest<ProductResponseDTO>
    {
    }
}
