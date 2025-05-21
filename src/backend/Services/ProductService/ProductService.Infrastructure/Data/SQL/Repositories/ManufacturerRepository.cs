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
    }
}
