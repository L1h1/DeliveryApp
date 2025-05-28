using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;

namespace OrderService.Infrastructure.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _mongoDb;

        public MongoDbContext(IMongoClient mongoClient)
        {
            _mongoDb = mongoClient.GetDatabase("products");

            ConfigureDb();
            ConfigureEntities();
        }

        public IMongoCollection<Order> Orders => _mongoDb.GetCollection<Order>("orders");

        private void ConfigureDb()
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
            ConventionRegistry.Register(
                "camelCase",
                new ConventionPack
                {
                    new CamelCaseElementNameConvention(),
                }, _ => true);
        }

        private void ConfigureEntities()
        {
            var collection = Orders;

            var indexKeysBuilder = Builders<Order>.IndexKeys;

            var indexModels = new List<CreateIndexModel<Order>>
            {
                new (
                    indexKeysBuilder.Ascending(o => o.ClientId),
                    new CreateIndexOptions { Name = "idx_clientId" }),

                new (
                    indexKeysBuilder.Ascending(o => o.CourierId)
                    .Descending(o => o.OrderStatus),
                    new CreateIndexOptions { Name = "idx_courierId_and_orderStatus" }),
            };

            collection.Indexes.CreateMany(indexModels);

            if (!BsonClassMap.IsClassMapRegistered(typeof(Order)))
            {
                BsonClassMap.RegisterClassMap<Order>(cm =>
                {
                    cm.AutoMap();
                    cm.MapMember(o => o.OrderStatus)
                      .SetSerializer(new EnumSerializer<OrderStatus>(BsonType.String));
                });
            }
        }
    }
}
