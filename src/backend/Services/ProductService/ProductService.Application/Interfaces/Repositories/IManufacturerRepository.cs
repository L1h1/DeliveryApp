using ProductService.Domain.Entities;

namespace ProductService.Application.Interfaces.Repositories
{
    public interface IManufacturerRepository : IBaseRepository<Manufacturer>
    {
        Task<Manufacturer> GetByNameAsync(string normalizedName, CancellationToken cancellationToken = default);
        Task<Manufacturer> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
