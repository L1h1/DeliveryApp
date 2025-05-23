using MediatR;

namespace ProductService.Application.Commands.ProductDetails.DeleteProductDetails
{
    public sealed record DeleteProductDetailsCommand(string id) : IRequest<Unit>
    {
    }
}
