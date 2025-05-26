using MediatR;

namespace ProductService.Application.Commands.ProductDetails.DeleteProductDetails
{
    public sealed record DeleteProductDetailsCommand(Guid id) : IRequest<Unit>
    {
    }
}
