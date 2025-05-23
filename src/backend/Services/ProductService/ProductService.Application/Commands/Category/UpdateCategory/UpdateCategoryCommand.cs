using MediatR;
using ProductService.Application.DTOs.Request;
using ProductService.Application.DTOs.Response;

namespace ProductService.Application.Commands.Category.UpdateCategory
{
    public sealed record UpdateCategoryCommand(string id, CategoryRequestDTO requestDTO) : IRequest<CategoryResponseDTO>
    {
    }
}
