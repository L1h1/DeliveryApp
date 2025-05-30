using MediatR;
using ProductService.Application.Exceptions;
using ProductService.Application.Interfaces.Repositories;

namespace ProductService.Application.Commands.Category.DeleteCategory
{
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Unit>
    {
        private readonly ICategoryRepository _categoryRepository;

        public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var existingCategory = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);

            if (existingCategory is null)
            {
                throw new NotFoundException("Category with given Id not found.");
            }

            await _categoryRepository.DeleteAsync(existingCategory, cancellationToken);

            return Unit.Value;
        }
    }
}
