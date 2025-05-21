using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
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
            cancellationToken.ThrowIfCancellationRequested();

            await _dbSet.AddAsync(tEntity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return tEntity;
        }

        public async Task DeleteAsync(T tEntity, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _dbSet.Remove(tEntity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<ICollection<T>> ListAsync(Expression<Func<T, bool>>? filter, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (filter is not null)
            {
                return await _dbSet.Where(filter).ToListAsync(cancellationToken);
            }
            else
            {
                return await _dbSet.ToListAsync(cancellationToken);
            }
        }

        public async Task<T?> UpdateAsync(T tEntity, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _dbSet.Update(tEntity);
            await _context.SaveChangesAsync(cancellationToken);

            return tEntity;
        }
    }
}
