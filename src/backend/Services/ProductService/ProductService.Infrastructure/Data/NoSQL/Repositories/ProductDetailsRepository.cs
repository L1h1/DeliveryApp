using System.Linq.Expressions;
using MongoDB.Driver;
using ProductService.Application.DTOs.Response;
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

        public async Task<ProductDetails> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Guid parsedId = Guid.Parse(id);
            var result = await _dbContext.ProductsDetails.Find(p => p.Id == parsedId).FirstOrDefaultAsync();

            return result;
        }

        public async Task<PaginatedResponseDTO<ProductDetails>> ListAsync(int currentPage, int pageSize, Expression<Func<ProductDetails, bool>>? filter, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _dbContext.ProductsDetails
                .Find(filter)
                .Skip((currentPage - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync(cancellationToken);
            var totalCount = await _dbContext.ProductsDetails.Find(filter).CountDocumentsAsync(cancellationToken);

            return new PaginatedResponseDTO<ProductDetails>
            {
                Items = result,
                TotalCount = totalCount,
                PageNumber = currentPage,
                PageSize = pageSize,
            };
        }

        public async Task<ProductDetails?> UpdateAsync(ProductDetails tEntity, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _dbContext.ProductsDetails.ReplaceOneAsync(p => p.Id == tEntity.Id, tEntity);

            return tEntity;
        }
    }
}
