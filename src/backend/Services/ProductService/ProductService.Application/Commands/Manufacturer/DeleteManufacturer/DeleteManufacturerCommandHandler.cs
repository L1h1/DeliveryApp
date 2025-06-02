using MediatR;
using ProductService.Application.Exceptions;
using ProductService.Application.Interfaces.Repositories;

namespace ProductService.Application.Commands.Manufacturer.DeleteManufacturer
{
    public class DeleteManufacturerCommandHandler : IRequestHandler<DeleteManufacturerCommand, Unit>
    {
        private readonly IManufacturerRepository _manufacturerRepository;

        public DeleteManufacturerCommandHandler(IManufacturerRepository manufacturerRepository)
        {
            _manufacturerRepository = manufacturerRepository;
        }

        public async Task<Unit> Handle(DeleteManufacturerCommand request, CancellationToken cancellationToken)
        {
            var existingManufacturer = await _manufacturerRepository.GetByIdAsync(request.Id, cancellationToken);

            if (existingManufacturer is null)
            {
                throw new NotFoundException("Manufacturer with given id not found.");
            }

            await _manufacturerRepository.DeleteAsync(existingManufacturer, cancellationToken);

            return Unit.Value;
        }
    }
}
