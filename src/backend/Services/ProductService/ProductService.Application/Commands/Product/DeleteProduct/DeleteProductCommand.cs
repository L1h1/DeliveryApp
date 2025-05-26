using MediatR;

namespace ProductService.Application.Commands.Product.DeleteProduct
{
    public sealed record DeleteProductCommand(Guid Id) : IRequest<Unit>
    {
    }
}
