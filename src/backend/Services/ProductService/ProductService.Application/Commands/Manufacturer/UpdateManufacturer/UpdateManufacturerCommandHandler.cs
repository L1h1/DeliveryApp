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

            _mapper.Map(request.requestDTO, existingManufacturer);

            existingManufacturer = await _manufacturerRepository.UpdateAsync(existingManufacturer, cancellationToken);

            return _mapper.Map<ManufacturerResponseDTO>(existingManufacturer);
        }
    }
}
