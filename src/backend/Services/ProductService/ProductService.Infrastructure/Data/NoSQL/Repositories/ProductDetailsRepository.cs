using System.Linq.Expressions;
using MongoDB.Driver;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Domain.Entities;

namespace ProductService.Infrastructure.Data.NoSQL.Repositories
{
    public class ProductDetailsRepository : IProductDetailsRepository
    {
        private readonly MongoDbContext _dbContext;

        public ProductDetailsRepository(MongoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ProductDetails?> AddAsync(ProductDetails tEntity, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _dbContext.ProductsDetails.InsertOneAsync(tEntity);

            return tEntity;
        }

        public async Task DeleteAsync(ProductDetails tEntity, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _dbContext.ProductsDetails.DeleteOneAsync(p => p.Id == tEntity.Id);
        }

        public async Task<ProductDetails> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _dbContext.ProductsDetails.Find(p => p.Id == id).FirstOrDefaultAsync();

            return result;
        }

        public async Task<ICollection<ProductDetails>> ListAsync(Expression<Func<ProductDetails, bool>>? filter, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _dbContext.ProductsDetails.Find(filter).ToListAsync(cancellationToken);

            return result;
        }

        public async Task<ProductDetails?> UpdateAsync(ProductDetails tEntity, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _dbContext.ProductsDetails.ReplaceOneAsync(p => p.Id == tEntity.Id, tEntity);

            return tEntity;
        }

    }
}
