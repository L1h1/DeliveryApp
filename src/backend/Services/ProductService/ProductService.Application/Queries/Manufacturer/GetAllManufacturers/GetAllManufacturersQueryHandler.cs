using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.DTOs.Response;
using ProductService.Application.Exceptions;
using ProductService.Application.Interfaces.Repositories;

namespace ProductService.Application.Queries.Manufacturer.GetAllManufacturers
{
    public class GetAllManufacturersQueryHandler : IRequestHandler<GetAllManufacturersQuery, PaginatedResponseDTO<ManufacturerResponseDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IManufacturerRepository _manufacturerRepository;
        private readonly ILogger<GetAllManufacturersQueryHandler> _logger;

        public GetAllManufacturersQueryHandler(
            IMapper mapper,
            IManufacturerRepository manufacturerRepository,
            ILogger<GetAllManufacturersQueryHandler> logger)
        {
            _mapper = mapper;
            _manufacturerRepository = manufacturerRepository;
            _logger = logger;
        }

        public async Task<PaginatedResponseDTO<ManufacturerResponseDTO>> Handle(GetAllManufacturersQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving manufacturer list page @{page}", request.Dto);

            var data = await _manufacturerRepository.ListAsync(request.Dto.PageNumber, request.Dto.PageSize, cancellationToken: cancellationToken);

            if (data.Items.Count == 0)
            {
                throw new NotFoundException("No manufacturers found.");
            }

            _logger.LogInformation("Successfully retrieved manufacturer list page @{page}", request.Dto);

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
