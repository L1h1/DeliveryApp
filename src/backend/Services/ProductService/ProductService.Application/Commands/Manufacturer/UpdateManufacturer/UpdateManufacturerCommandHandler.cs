using AutoMapper;
using MediatR;
using ProductService.Application.DTOs.Response;
using ProductService.Application.Exceptions;
using ProductService.Application.Interfaces.Repositories;

namespace ProductService.Application.Commands.Manufacturer.UpdateManufacturer
{
    public class UpdateManufacturerCommandHandler : IRequestHandler<UpdateManufacturerCommand, ManufacturerResponseDTO>
    {
        private readonly IMapper _mapper;
        private readonly IManufacturerRepository _manufacturerRepository;

        public UpdateManufacturerCommandHandler(IMapper mapper, IManufacturerRepository manufacturerRepository)
        {
            _mapper = mapper;
            _manufacturerRepository = manufacturerRepository;
        }

        public async Task<ManufacturerResponseDTO> Handle(UpdateManufacturerCommand request, CancellationToken cancellationToken)
        {
            var existingManufacturer = await _manufacturerRepository.GetByIdAsync(request.id, cancellationToken);

            if (existingManufacturer is null)
            {
                throw new NotFoundException("Manufacturer with given id not found.");
            }

            var normalizedName = request.requestDTO.Name.Trim().ToLower();
            var manufacturerWithSameName = await _manufacturerRepository.GetByNameAsync(normalizedName, cancellationToken);

            if (manufacturerWithSameName is not null && manufacturerWithSameName.Id != existingManufacturer.Id)
            {
                throw new BadRequestException("Another manufacturer with given name already exists.");
            }

            _mapper.Map(request.requestDTO, existingManufacturer);

            existingManufacturer = await _manufacturerRepository.UpdateAsync(existingManufacturer, cancellationToken);

            return _mapper.Map<ManufacturerResponseDTO>(existingManufacturer);
        }
    }
}
