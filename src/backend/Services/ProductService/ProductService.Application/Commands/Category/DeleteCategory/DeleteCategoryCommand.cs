using MediatR;

namespace ProductService.Application.Commands.Category.DeleteCategory
{
    public sealed record DeleteCategoryCommand(Guid id) : IRequest<Unit>
    {
    }
}
