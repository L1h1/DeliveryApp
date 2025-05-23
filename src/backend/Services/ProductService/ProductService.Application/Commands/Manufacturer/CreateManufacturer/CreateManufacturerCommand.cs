using MediatR;
using ProductService.Application.DTOs.Request;
using ProductService.Application.DTOs.Response;

namespace ProductService.Application.Commands.Manufacturer.CreateManufacturer
{
    public sealed record CreateManufacturerCommand(ManufacturerRequestDTO requestDTO) : IRequest<ManufacturerResponseDTO>
    {
    }
}
