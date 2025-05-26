using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Components;
using ProductService.Application.DTOs.Response;
using ProductService.Application.Exceptions;
using ProductService.Application.Interfaces.Repositories;

namespace ProductService.Application.Commands.Category.UpdateCategory
{
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryResponseDTO>
    {
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _categoryRepository;

        public UpdateCategoryCommandHandler(IMapper mapper, ICategoryRepository categoryRepository)
        {
            _mapper = mapper;
            _categoryRepository = categoryRepository;
        }

        public async Task<CategoryResponseDTO> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var existingCategory = await _categoryRepository.GetByIdAsync(request.id, cancellationToken);

            if (existingCategory is null)
            {
                throw new NotFoundException("Category with given id not found.");
            }

            var normalizedName = request.requestDTO.Name.Trim().ToLower();
            var categoryWithSameName = await _categoryRepository.GetByNameAsync(normalizedName, cancellationToken);

            if (categoryWithSameName is not null && categoryWithSameName.Id != existingCategory.Id)
            {
                throw new BadRequestException("Another category with this name already exists.");
            }

            _mapper.Map(request.requestDTO, existingCategory);

            existingCategory = await _categoryRepository.UpdateAsync(existingCategory, cancellationToken);

            return _mapper.Map<CategoryResponseDTO>(existingCategory);
        }
    }
}
