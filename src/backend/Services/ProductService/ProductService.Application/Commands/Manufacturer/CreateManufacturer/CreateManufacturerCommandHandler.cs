using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.DTOs.Response;
using ProductService.Application.Exceptions;
using ProductService.Application.Interfaces.Repositories;

namespace ProductService.Application.Commands.Manufacturer.CreateManufacturer
{
    public class CreateManufacturerCommandHandler : IRequestHandler<CreateManufacturerCommand, ManufacturerResponseDTO>
    {
        private readonly IMapper _mapper;
        private readonly IManufacturerRepository _manufacturerRepository;
        private readonly ILogger<CreateManufacturerCommandHandler> _logger;

        public CreateManufacturerCommandHandler(
            IMapper mapper,
            IManufacturerRepository manufacturerRepository,
            ILogger<CreateManufacturerCommandHandler> logger)
        {
            _mapper = mapper;
            _manufacturerRepository = manufacturerRepository;
            _logger = logger;
        }

        public async Task<ManufacturerResponseDTO> Handle(CreateManufacturerCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating manufacturer {@manufacturer}", request.RequestDTO);

            var existingManufacturer = await _manufacturerRepository.GetByNameAsync(request.RequestDTO.Name.ToLower(), cancellationToken);

            if (existingManufacturer is not null)
            {
                throw new BadRequestException("Manufacturer with given name already exists.");
            }

            var manufacturer = _mapper.Map<Domain.Entities.Manufacturer>(request.RequestDTO);

            manufacturer = await _manufacturerRepository.AddAsync(manufacturer, cancellationToken);

            _logger.LogInformation("Created manufacturer {@id}", manufacturer.Id);

            return _mapper.Map<ManufacturerResponseDTO>(manufacturer);
        }
    }
}
