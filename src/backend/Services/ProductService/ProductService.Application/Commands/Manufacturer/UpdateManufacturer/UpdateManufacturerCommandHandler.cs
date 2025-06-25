using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.DTOs.Response;
using ProductService.Application.Exceptions;
using ProductService.Application.Interfaces.Repositories;

namespace ProductService.Application.Commands.Manufacturer.UpdateManufacturer
{
    public class UpdateManufacturerCommandHandler : IRequestHandler<UpdateManufacturerCommand, ManufacturerResponseDTO>
    {
        private readonly IMapper _mapper;
        private readonly IManufacturerRepository _manufacturerRepository;
        private readonly ILogger<UpdateManufacturerCommandHandler> _logger;

        public UpdateManufacturerCommandHandler(
            IMapper mapper,
            IManufacturerRepository manufacturerRepository,
            ILogger<UpdateManufacturerCommandHandler> logger)
        {
            _mapper = mapper;
            _manufacturerRepository = manufacturerRepository;
            _logger = logger;
        }

        public async Task<ManufacturerResponseDTO> Handle(UpdateManufacturerCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating manufacturer @{id}", request.Id);

            var existingManufacturer = await _manufacturerRepository.GetByIdAsync(request.Id, cancellationToken);

            if (existingManufacturer is null)
            {
                throw new NotFoundException("Manufacturer with given id not found.");
            }

            _logger.LogInformation("Searching for manufacturer with same data @{name}", request.RequestDTO.Name);

            var normalizedName = request.RequestDTO.Name.Trim().ToLower();
            var manufacturerWithSameName = await _manufacturerRepository.GetByNameAsync(normalizedName, cancellationToken);

            if (manufacturerWithSameName is not null && manufacturerWithSameName.Id != existingManufacturer.Id)
            {
                throw new BadRequestException("Another manufacturer with given name already exists.");
            }

            _mapper.Map(request.RequestDTO, existingManufacturer);

            existingManufacturer = await _manufacturerRepository.UpdateAsync(existingManufacturer, cancellationToken);

            _logger.LogInformation("Successfully updated manufacturer @{id}", request.Id);

            return _mapper.Map<ManufacturerResponseDTO>(existingManufacturer);
        }
    }
}
