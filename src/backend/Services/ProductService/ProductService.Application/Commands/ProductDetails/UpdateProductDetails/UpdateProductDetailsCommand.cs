using MediatR;
using ProductService.Application.DTOs.Request;
using ProductService.Application.DTOs.Response;

namespace ProductService.Application.Commands.ProductDetails.UpdateProductDetails
{
    public sealed record UpdateProductDetailsCommand(ProductDetailsRequestDTO RequestDTO) : IRequest<ProductDetailsResponseDTO>
    {
    }
}
