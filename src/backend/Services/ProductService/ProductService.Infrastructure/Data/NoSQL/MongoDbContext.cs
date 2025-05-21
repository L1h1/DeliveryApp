using MongoDB.Driver;
using ProductService.Domain.Entities;

namespace ProductService.Infrastructure.Data.NoSQL
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _mongoDb;

        public MongoDbContext(IMongoClient mongoClient)
        {
            _mongoDb = mongoClient.GetDatabase("products");
        }

        public IMongoCollection<ProductDetails> ProductsDetails => _mongoDb.GetCollection<ProductDetails>("productsDetails");
    }
}
