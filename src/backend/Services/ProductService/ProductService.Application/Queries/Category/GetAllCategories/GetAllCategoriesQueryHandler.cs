using System.Text.Json;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Distributed;
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
        private readonly IDistributedCache _distributedCache;

        public GetAllCategoriesQueryHandler(IMapper mapper, ICategoryRepository categoryRepository, ILogger<GetAllCategoriesQueryHandler> logger, IDistributedCache distributedCache)
        {
            _mapper = mapper;
            _categoryRepository = categoryRepository;
            _logger = logger;
            _distributedCache = distributedCache;
        
        }

        public async Task<PaginatedResponseDTO<CategoryResponseDTO>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving category list page @{page}", request.Dto);
            
            var cacheKey = $"categories:{request.Dto.PageNumber}:{request.Dto.PageSize}";
            var cachedData = await _distributedCache.GetStringAsync(cacheKey, cancellationToken);

            if (!string.IsNullOrEmpty(cachedData))
            {
                return JsonSerializer.Deserialize<PaginatedResponseDTO<CategoryResponseDTO>>(cachedData);
            }

            var data = await _categoryRepository.ListAsync(request.Dto.PageNumber, request.Dto.PageSize, cancellationToken: cancellationToken);

            if (data.Items.Count == 0)
            {
                throw new NotFoundException("No categories found");
            }

            _logger.LogInformation("Successfully retrieved category list page @{page}", request.Dto);

            var result = new PaginatedResponseDTO<CategoryResponseDTO>

            {
                Items = _mapper.Map<ICollection<CategoryResponseDTO>>(data.Items),
                PageNumber = data.PageNumber,
                PageSize = data.PageSize,
                TotalCount = data.TotalCount,
            };

            await _distributedCache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(result),
                new DistributedCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                }, cancellationToken);

            return result;
        }
    }
}
