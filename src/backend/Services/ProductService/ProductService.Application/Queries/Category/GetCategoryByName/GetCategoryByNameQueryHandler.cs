using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.DTOs.Response;
using ProductService.Application.Exceptions;
using ProductService.Application.Interfaces.Repositories;

namespace ProductService.Application.Queries.Category.GetCategoryByName
{
    public class GetCategoryByNameQueryHandler : IRequestHandler<GetCategoryByNameQuery, CategoryResponseDTO>
    {
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<GetCategoryByNameQueryHandler> _logger;

        public GetCategoryByNameQueryHandler(IMapper mapper, ICategoryRepository categoryRepository, ILogger<GetCategoryByNameQueryHandler> logger)
        {
            _mapper = mapper;
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        public async Task<CategoryResponseDTO> Handle(GetCategoryByNameQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving category data with @{name}", request.Name);

            var data = await _categoryRepository.GetByNameAsync(request.Name, cancellationToken);

            if (data is null)
            {
                throw new NotFoundException("Category not found");
            }

            _logger.LogInformation("Successfully retrieved category data with @{name}", request.Name);

            return _mapper.Map<CategoryResponseDTO>(data);
        }
    }
}
