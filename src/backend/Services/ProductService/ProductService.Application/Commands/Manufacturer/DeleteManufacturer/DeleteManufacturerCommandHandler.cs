using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.Exceptions;
using ProductService.Application.Interfaces.Repositories;

namespace ProductService.Application.Commands.Manufacturer.DeleteManufacturer
{
    public class DeleteManufacturerCommandHandler : IRequestHandler<DeleteManufacturerCommand, Unit>
    {
        private readonly IManufacturerRepository _manufacturerRepository;
        private readonly ILogger<DeleteManufacturerCommandHandler> _logger;

        public DeleteManufacturerCommandHandler(IManufacturerRepository manufacturerRepository, ILogger<DeleteManufacturerCommandHandler> logger)
        {
            _manufacturerRepository = manufacturerRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteManufacturerCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting manufacturer @{id}", request.Id);

            var existingManufacturer = await _manufacturerRepository.GetByIdAsync(request.Id, cancellationToken);

            if (existingManufacturer is null)
            {
                throw new NotFoundException("Manufacturer with given id not found.");
            }

            await _manufacturerRepository.DeleteAsync(existingManufacturer, cancellationToken);

            _logger.LogInformation("Successfully deleted manufacturer @{id}", request.Id);

            return Unit.Value;
        }
    }
}
