using MediatR;

namespace ProductService.Application.Commands.ProductDetails.DeleteProductDetails
{
    public sealed record DeleteProductDetailsCommand(Guid Id) : IRequest<Unit>
    {
    }
}
