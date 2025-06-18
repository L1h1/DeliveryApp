using Microsoft.EntityFrameworkCore;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Application.Interfaces.Services;
using ProductService.Domain.Entities;

namespace ProductService.Infrastructure.Data.SQL.Repositories
{
    public class CategoryRepository : EFBaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(EFDbContext context, ICacheService cache)
            : base(context, cache)
        {
        }

        public async Task<Category?> GetByNameAsync(string normalizedName, CancellationToken cancellationToken = default)
        {
            var result = await _dbSet.FirstOrDefaultAsync(c => c.NormalizedName == normalizedName, cancellationToken);

            return result;
        }

        public async Task<ICollection<Category>>? ListByIdsAsync(ICollection<Guid> ids, CancellationToken cancellationToken = default)
        {
            var result = await _dbSet.Where(c => ids.Contains(c.Id)).ToListAsync(cancellationToken);

            return result;
        }
    }
}
