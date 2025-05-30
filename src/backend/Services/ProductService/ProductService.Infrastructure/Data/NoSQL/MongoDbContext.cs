using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
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

            Configure();
        }

        public IMongoCollection<ProductDetails> ProductsDetails => _mongoDb.GetCollection<ProductDetails>("productsDetails");

        public void Configure()
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
            ConventionRegistry.Register(
                "camelCase",
                new ConventionPack
                {
                    new CamelCaseElementNameConvention(),
                }, _ => true);
        }
    }
}
