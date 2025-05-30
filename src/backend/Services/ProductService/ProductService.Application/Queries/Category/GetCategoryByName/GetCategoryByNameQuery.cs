using MediatR;
using ProductService.Application.DTOs.Response;

namespace ProductService.Application.Queries.Category.GetCategoryByName
{
    public sealed record GetCategoryByNameQuery(string Name) : IRequest<CategoryResponseDTO>
    {
    }
}
