using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.DTOs.Response;
using ProductService.Application.Exceptions;
using ProductService.Application.Interfaces.Repositories;

namespace ProductService.Application.Commands.Category.CreateCategory
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryResponseDTO>
    {
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<CreateCategoryCommandHandler> _logger;

        public CreateCategoryCommandHandler(IMapper mapper, ICategoryRepository categoryRepository, ILogger<CreateCategoryCommandHandler> logger)
        {
            _mapper = mapper;
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        public async Task<CategoryResponseDTO> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing creation of new category @{name}", request.RequestDTO.Name);

            var normalizedName = request.RequestDTO.Name.Trim().ToLower();
            var existingCategory = await _categoryRepository.GetByNameAsync(normalizedName, cancellationToken);

            if (existingCategory is not null)
            {
                throw new BadRequestException("Category with given name already exists.");
            }

            var category = _mapper.Map<Domain.Entities.Category>(request.RequestDTO);

            await _categoryRepository.AddAsync(category, cancellationToken);

            _logger.LogInformation("Successfully created new category @{id}", category.Id);

            return _mapper.Map<CategoryResponseDTO>(category);
        }
    }
}
