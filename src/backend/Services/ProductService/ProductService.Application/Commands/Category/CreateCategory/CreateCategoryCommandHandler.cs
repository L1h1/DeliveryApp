using AutoMapper;
using MediatR;
using ProductService.Application.DTOs.Response;
using ProductService.Application.Exceptions;
using ProductService.Application.Interfaces.Repositories;

namespace ProductService.Application.Commands.Category.CreateCategory
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryResponseDTO>
    {
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _categoryRepository;

        public CreateCategoryCommandHandler(IMapper mapper, ICategoryRepository categoryRepository)
        {
            _mapper = mapper;
            _categoryRepository = categoryRepository;
        }

        public async Task<CategoryResponseDTO> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var normalizedName = request.RequestDTO.Name.Trim().ToLower();
            var existingCategory = await _categoryRepository.GetByNameAsync(normalizedName, cancellationToken);

            if (existingCategory is not null)
            {
                throw new BadRequestException("Category with given name already exists.");
            }

            var category = _mapper.Map<Domain.Entities.Category>(request.RequestDTO);

            await _categoryRepository.AddAsync(category, cancellationToken);

            return _mapper.Map<CategoryResponseDTO>(category);
        }
    }
}
