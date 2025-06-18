using Microsoft.EntityFrameworkCore;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Application.Interfaces.Services;
using ProductService.Domain.Entities;

namespace ProductService.Infrastructure.Data.SQL.Repositories
{
    public class ManufacturerRepository : EFBaseRepository<Manufacturer>, IManufacturerRepository
    {
        public ManufacturerRepository(EFDbContext context, ICacheService cache)
            : base(context, cache)
        {
        }

        public async Task<Manufacturer> GetByNameAsync(string normalizedName, CancellationToken cancellationToken = default)
        {
            var result = await _dbSet.FirstOrDefaultAsync(m => m.NormalizedName == normalizedName, cancellationToken);

            return result;
        }
    }
}
