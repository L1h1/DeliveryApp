using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ProductService.Application.DTOs.Response;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Application.Interfaces.Services;
using ProductService.Domain.Entities;

namespace ProductService.Infrastructure.Data.SQL.Repositories
{
    public class EFBaseRepository<T> : IBaseRepository<T>
        where T : BaseEntity
    {
        private protected readonly DbSet<T> _dbSet;
        private protected readonly EFDbContext _context;
        private protected readonly ICacheService _cacheService;

        public EFBaseRepository(EFDbContext context, ICacheService cacheService)
        {
            _context = context;
            _dbSet = _context.Set<T>();
            _cacheService = cacheService;
        }

        public async Task<T?> AddAsync(T tEntity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(tEntity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            await _cacheService.SetAsync($"{typeof(T).Name}:{tEntity.Id}", tEntity, cancellationToken);

            return tEntity;
        }

        public async Task DeleteAsync(T tEntity, CancellationToken cancellationToken = default)
        {
            _dbSet.Remove(tEntity);
            await _context.SaveChangesAsync(cancellationToken);
            await _cacheService.RemoveAsync($"{typeof(T).Name}:{tEntity.Id}", cancellationToken);
        }

        public async virtual Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var key = $"{typeof(T).Name}:{id}";
            var cached = await _cacheService.GetAsync<T>(key, cancellationToken);

            if (cached is not null)
            {
                return cached;
            }

            var data = await _dbSet.FindAsync(id, cancellationToken);

            if (data is not null)
            {
                await _cacheService.SetAsync(key, data, cancellationToken);
            }

            return data;
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

            var key = $"{typeof(T).Name}:{tEntity.Id}";

            await _cacheService.RemoveAsync(key, cancellationToken);
            await _cacheService.SetAsync(key, tEntity, cancellationToken);

            return tEntity;
        }
    }
}
