using MediatR;
using ProductService.Application.DTOs.Request;
using ProductService.Application.DTOs.Response;

namespace ProductService.Application.Commands.Product.CreateProduct
{
    public sealed record CreateProductCommand(ProductRequestDTO requestDTO) : IRequest<ProductResponseDTO>
    {
    }
}
