using MediatR;
using ProductService.Application.DTOs.Request;
using ProductService.Application.DTOs.Response;

namespace ProductService.Application.Commands.Category.CreateCategory
{
    public sealed record CreateCategoryCommand(CategoryRequestDTO requestDTO) : IRequest<CategoryResponseDTO>
    {
    }
}
