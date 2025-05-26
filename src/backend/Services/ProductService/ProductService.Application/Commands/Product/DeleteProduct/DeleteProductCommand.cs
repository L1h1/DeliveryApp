using MediatR;

namespace ProductService.Application.Commands.Product.DeleteProduct
{
    public sealed record DeleteProductCommand(Guid id) : IRequest<Unit>
    {
    }
}
