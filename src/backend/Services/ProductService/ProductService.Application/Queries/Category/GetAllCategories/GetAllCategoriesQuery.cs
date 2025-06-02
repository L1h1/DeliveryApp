using MediatR;
using ProductService.Application.DTOs.Request;
using ProductService.Application.DTOs.Response;

namespace ProductService.Application.Queries.Category.GetAllCategories
{
    public sealed record GetAllCategoriesQuery(PageRequestDTO Dto) : IRequest<PaginatedResponseDTO<CategoryResponseDTO>> { }
}
