using AutoMapper;
using MediatR;
using ProductService.Application.DTOs.Response;
using ProductService.Application.Exceptions;
using ProductService.Application.Interfaces.Repositories;

namespace ProductService.Application.Commands.Manufacturer.CreateManufacturer
{
    public class CreateManufacturerCommandHandler : IRequestHandler<CreateManufacturerCommand, ManufacturerResponseDTO>
    {
        private readonly IMapper _mapper;
        private readonly IManufacturerRepository _manufacturerRepository;

        public CreateManufacturerCommandHandler(IMapper mapper, IManufacturerRepository manufacturerRepository)
        {
            _mapper = mapper;
            _manufacturerRepository = manufacturerRepository;
        }

        public async Task<ManufacturerResponseDTO> Handle(CreateManufacturerCommand request, CancellationToken cancellationToken)
        {
            var existingManufacturer = await _manufacturerRepository.GetByNameAsync(request.requestDTO.Name.ToLower());

            if (existingManufacturer is not null)
            {
                throw new BadRequestException("Manufacturer with given name already exists.");
            }

            var manufacturer = _mapper.Map<Domain.Entities.Manufacturer>(request.requestDTO);

            manufacturer = await _manufacturerRepository.AddAsync(manufacturer, cancellationToken);

            return _mapper.Map<ManufacturerResponseDTO>(manufacturer);
        }
    }
}
