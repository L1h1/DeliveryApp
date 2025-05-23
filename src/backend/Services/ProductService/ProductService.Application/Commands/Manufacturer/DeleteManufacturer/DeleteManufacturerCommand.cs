using MediatR;

namespace ProductService.Application.Commands.Manufacturer.DeleteManufacturer
{
    public sealed record DeleteManufacturerCommand(string id) : IRequest<Unit>
    {
    }
}
