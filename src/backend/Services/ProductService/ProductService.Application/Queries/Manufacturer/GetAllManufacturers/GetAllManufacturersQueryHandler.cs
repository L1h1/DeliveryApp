using AutoMapper;
using MediatR;
using ProductService.Application.DTOs.Response;
using ProductService.Application.Exceptions;
using ProductService.Application.Interfaces.Repositories;

namespace ProductService.Application.Queries.Manufacturer.GetAllManufacturers
{
    public class GetAllManufacturersQueryHandler : IRequestHandler<GetAllManufacturersQuery, PaginatedResponseDTO<ManufacturerResponseDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IManufacturerRepository _manufacturerRepository;

        public GetAllManufacturersQueryHandler(IMapper mapper, IManufacturerRepository manufacturerRepository)
        {
            _mapper = mapper;
            _manufacturerRepository = manufacturerRepository;
        }

        public async Task<PaginatedResponseDTO<ManufacturerResponseDTO>> Handle(GetAllManufacturersQuery request, CancellationToken cancellationToken)
        {
            var data = await _manufacturerRepository.ListAsync(request.Dto.PageNumber, request.Dto.PageSize, cancellationToken: cancellationToken);

            if (data.Items.Count == 0)
            {
                throw new NotFoundException("No manufacturers found.");
            }

            return new PaginatedResponseDTO<ManufacturerResponseDTO>
            {
                Items = _mapper.Map<ICollection<ManufacturerResponseDTO>>(data.Items),
                PageNumber = data.PageNumber,
                PageSize = data.PageSize,
                TotalCount = data.TotalCount,
            };
        }
    }
}
