using MediatR;
using ProductService.Application.DTOs.Request;
using ProductService.Application.DTOs.Response;

namespace ProductService.Application.Commands.ProductDetails.CreateProductDetails
{
    public sealed record CreateProductDetailsCommand(ProductDetailsRequestDTO requestDTO) : IRequest<ProductDetailsResponseDTO>
    {
    }
}
