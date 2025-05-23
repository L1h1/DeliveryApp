using MediatR;

namespace ProductService.Application.Commands.Product.DeleteProduct
{
    public sealed record DeleteProductCommand(string id) : IRequest<Unit>
    {
    }
}
