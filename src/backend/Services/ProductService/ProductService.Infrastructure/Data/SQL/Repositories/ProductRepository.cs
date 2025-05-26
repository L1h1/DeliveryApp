using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ProductService.Application.DTOs.Response;
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

        public async Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _dbSet
                .Include(p => p.Manufacturer)
                .Include(p => p.Categories)
                .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);

            return result;
        }

        public async Task<PaginatedResponseDTO<Product>> ListWithNestedAsync(int pageNumber, int pageSize, Expression<Func<Product, bool>>? filter = null, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var data = new List<Product>();
            var query = _dbSet
                .Include(p => p.Manufacturer)
                .Include(p => p.Categories) as IQueryable<Product>;

            int totalCount = default;

            if (filter is null)
            {
                totalCount = await query.CountAsync(cancellationToken);
                data = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);
            }
            else
            {
                query = query.Where(filter);
                totalCount = await query.CountAsync(cancellationToken);

                data = await query
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync(cancellationToken);
            }

            return new PaginatedResponseDTO<Product>
            {
                Items = data,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
            };
        }
    }
}
