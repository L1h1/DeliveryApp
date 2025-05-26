using MediatR;
using ProductService.Application.DTOs.Request;
using ProductService.Application.DTOs.Response;

namespace ProductService.Application.Queries.Manufacturer.GetAllManufacturers
{
    public sealed record GetAllManufacturersQuery(PageRequestDTO Dto) : IRequest<PaginatedResponseDTO<ManufacturerResponseDTO>>
    {
    }
}
