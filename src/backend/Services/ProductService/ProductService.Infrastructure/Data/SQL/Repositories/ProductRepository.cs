using Microsoft.EntityFrameworkCore;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Domain.Entities;

namespace ProductService.Infrastructure.Data.SQL.Repositories
{
    public class ProductRepository : EFBaseRepository<Product>, IProductRepository
    {
        public ProductRepository(EFDbContext context)
            : base(context)
        {
        }

        public async Task<Product?> GetByIdAsync(string productId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _dbSet
                .Include(p => p.Manufacturer)
                .Include(p => p.Categories)
                .FirstOrDefaultAsync(p => p.Id == Guid.Parse(productId), cancellationToken);

            return result;
        }

        public async Task<ICollection<Product>> ListWithQueryAsync(Func<IQueryable<Product>, IQueryable<Product>>? query, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (query is null)
            {
                return await _dbSet.ToListAsync(cancellationToken);
            }

            return await query(_dbSet).ToListAsync(cancellationToken);
        }
    }
}
