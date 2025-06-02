using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ProductService.Application.DTOs.Response;
using ProductService.Application.Interfaces.Repositories;

namespace ProductService.Infrastructure.Data.SQL.Repositories
{
    public class EFBaseRepository<T> : IBaseRepository<T>
        where T : class
    {
        protected readonly DbSet<T> _dbSet;
        protected readonly EFDbContext _context;

        public EFBaseRepository(EFDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<T?> AddAsync(T tEntity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(tEntity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return tEntity;
        }

        public async Task DeleteAsync(T tEntity, CancellationToken cancellationToken = default)
        {
            _dbSet.Remove(tEntity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<PaginatedResponseDTO<T>> ListAsync(int pageNumber, int pageSize, Expression<Func<T, bool>>? filter = null, CancellationToken cancellationToken = default)
        {
            List<T> data;
            int totalCount;

            if (filter is not null)
            {
                data = await _dbSet
                    .Where(filter)
                    .Skip((pageNumber - 1 ) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);

                totalCount = await _dbSet.Where(filter).CountAsync(cancellationToken);
            }
            else
            {
                data = await _dbSet
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);

                totalCount = await _dbSet.CountAsync(cancellationToken);
            }

            return new PaginatedResponseDTO<T>
            {
                Items = data,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
            };
        }

        public async Task<T?> UpdateAsync(T tEntity, CancellationToken cancellationToken = default)
        {
            _dbSet.Update(tEntity);
            await _context.SaveChangesAsync(cancellationToken);

            return tEntity;
        }
    }
}
