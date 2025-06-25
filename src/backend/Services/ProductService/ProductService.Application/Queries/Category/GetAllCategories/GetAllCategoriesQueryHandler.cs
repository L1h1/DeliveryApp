using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.DTOs.Response;
using ProductService.Application.Exceptions;
using ProductService.Application.Interfaces.Repositories;

namespace ProductService.Application.Queries.Category.GetAllCategories
{
    public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, PaginatedResponseDTO<CategoryResponseDTO>>
    {
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<GetAllCategoriesQueryHandler> _logger;

        public GetAllCategoriesQueryHandler(IMapper mapper, ICategoryRepository categoryRepository, ILogger<GetAllCategoriesQueryHandler> logger)
        {
            _mapper = mapper;
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        public async Task<PaginatedResponseDTO<CategoryResponseDTO>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving category list page @{page}", request.Dto);

            var data = await _categoryRepository.ListAsync(request.Dto.PageNumber, request.Dto.PageSize, cancellationToken: cancellationToken);

            if (data.Items.Count == 0)
            {
                throw new NotFoundException("No categories found");
            }

            _logger.LogInformation("Successfully retrieved category list page @{page}", request.Dto);

            return new PaginatedResponseDTO<CategoryResponseDTO>
            {
                Items = _mapper.Map<ICollection<CategoryResponseDTO>>(data.Items),
                PageNumber = data.PageNumber,
                PageSize = data.PageSize,
                TotalCount = data.TotalCount,
            };
        }
    }
}
