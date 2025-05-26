using AutoMapper;
using MediatR;
using ProductService.Application.DTOs.Response;
using ProductService.Application.Exceptions;
using ProductService.Application.Interfaces.Repositories;

namespace ProductService.Application.Queries.Category.GetCategoryByName
{
    public class GetCategoryByNameQueryHandler : IRequestHandler<GetCategoryByNameQuery, CategoryResponseDTO>
    {
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _categoryRepository;

        public GetCategoryByNameQueryHandler(IMapper mapper, ICategoryRepository categoryRepository)
        {
            _mapper = mapper;
            _categoryRepository = categoryRepository;
        }

        public async Task<CategoryResponseDTO> Handle(GetCategoryByNameQuery request, CancellationToken cancellationToken)
        {
            var data = await _categoryRepository.GetByNameAsync(request.Name, cancellationToken);

            if (data is null)
            {
                throw new NotFoundException("Category not found");
            }

            return _mapper.Map<CategoryResponseDTO>(data);
        }
    }
}
