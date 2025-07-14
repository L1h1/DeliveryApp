using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using ProductService.Application.DTOs.Response;
using ProductService.Application.Exceptions;
using ProductService.Application.Interfaces.Repositories;

namespace ProductService.Application.Commands.Category.UpdateCategory
{
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryResponseDTO>
    {
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<UpdateCategoryCommandHandler> _logger;

        public UpdateCategoryCommandHandler(IMapper mapper, ICategoryRepository categoryRepository, ILogger<UpdateCategoryCommandHandler> logger)
        {
            _mapper = mapper;
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        public async Task<CategoryResponseDTO> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving existing category @{id}", request.Id);

            var existingCategory = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);

            if (existingCategory is null)
            {
                throw new NotFoundException("Category with given id not found.");
            }

            _logger.LogInformation("Checking for another category with given data @{name}", request.RequestDTO.Name);

            var normalizedName = request.RequestDTO.Name.Trim().ToLower();
            var categoryWithSameName = await _categoryRepository.GetByNameAsync(normalizedName, cancellationToken);

            if (categoryWithSameName is not null && categoryWithSameName.Id != existingCategory.Id)
            {
                throw new BadRequestException("Another category with this name already exists.");
            }

            _mapper.Map(request.RequestDTO, existingCategory);

            existingCategory = await _categoryRepository.UpdateAsync(existingCategory, cancellationToken);

            _logger.LogInformation("Successfully updated category @{id}", request.Id);

            return _mapper.Map<CategoryResponseDTO>(existingCategory);
        }
    }
}
