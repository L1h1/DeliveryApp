using MediatR;
using ProductService.Application.DTOs.Request;
using ProductService.Application.DTOs.Response;

namespace ProductService.Application.Commands.Manufacturer.UpdateManufacturer
{
    public sealed record UpdateManufacturerCommand(Guid Id, ManufacturerRequestDTO RequestDTO) : IRequest<ManufacturerResponseDTO>
    {
    }
}
