using MediatR;

namespace ProductService.Application.Commands.Manufacturer.DeleteManufacturer
{
    public sealed record DeleteManufacturerCommand(Guid Id) : IRequest<Unit>
    {
    }
}
