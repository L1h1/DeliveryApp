using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.Exceptions;
using ProductService.Application.Interfaces.Repositories;

namespace ProductService.Application.Commands.Category.DeleteCategory
{
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Unit>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<DeleteCategoryCommandHandler> _logger;

        public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository, ILogger<DeleteCategoryCommandHandler> logger)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing deletion of category @{id}", request.Id);

            var existingCategory = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);

            if (existingCategory is null)
            {
                throw new NotFoundException("Category with given Id not found.");
            }

            await _categoryRepository.DeleteAsync(existingCategory, cancellationToken);

            _logger.LogInformation("Successfully deleted category @{id}", request.Id);

            return Unit.Value;
        }
    }
}
