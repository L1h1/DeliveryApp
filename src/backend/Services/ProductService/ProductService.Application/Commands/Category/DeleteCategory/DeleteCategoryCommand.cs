using MediatR;

namespace ProductService.Application.Commands.Category.DeleteCategory
{
    public sealed record DeleteCategoryCommand(string id) : IRequest<Unit>
    {
    }
}
