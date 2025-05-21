using Microsoft.EntityFrameworkCore;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Domain.Entities;

namespace ProductService.Infrastructure.Data.SQL.Repositories
{
    public class CategoryRepository : EFBaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(EFDbContext context)
            : base(context)
        {
        }

        public async Task<Category?> GetByNameAsync(string categoryName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _dbSet.FirstOrDefaultAsync(c => c.Name == categoryName, cancellationToken);

            return result;
        }
    }
}
