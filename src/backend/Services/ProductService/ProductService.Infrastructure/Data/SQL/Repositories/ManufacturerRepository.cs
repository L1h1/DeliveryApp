using Microsoft.EntityFrameworkCore;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Domain.Entities;

namespace ProductService.Infrastructure.Data.SQL.Repositories
{
    public class ManufacturerRepository : EFBaseRepository<Manufacturer>, IManufacturerRepository
    {
        public ManufacturerRepository(EFDbContext context)
            : base(context)
        {
        }

        public async Task<Manufacturer> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _dbSet.FindAsync(id, cancellationToken);

            return result;
        }

        public async Task<Manufacturer> GetByNameAsync(string normalizedName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _dbSet.FirstOrDefaultAsync(m => m.NormalizedName == normalizedName);

            return result;
        }
    }
}
