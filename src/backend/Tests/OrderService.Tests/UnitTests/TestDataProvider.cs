using OrderService.Application.DTOs.Request;
using OrderService.Domain.Entities;

namespace OrderService.Tests.UnitTests
{
    public class TestDataProvider
    {
        private static Random _random = new Random();

        public static OrderRequestDTO SampleOrderRequestDTO => new()
        {
            ClientId = Guid.NewGuid(),
            Address = string.Empty,
            Items = Enumerable.Range(1, 10).Select(i => SampleOrderItemRequestDTO).ToList(),
            ClientComment = "TEST",
        };

        public static OrderItemRequestDTO SampleOrderItemRequestDTO => new()
        {
            ProductId = Guid.NewGuid(),
            Quantity = _random.Next(1, 10)
        };

        public static Order SampleOrderEntity => new()
        {
            Id = Guid.NewGuid(),
            ClientId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            OrderStatus = Domain.Enums.OrderStatus.Created,
            ClientComment = "TEST",
            Address = "TEST",
            Items = Enumerable.Range(1, _random.Next(10)).Select(i => SampleOrderItemEntity).ToList(),
        };

        public static OrderItem SampleOrderItemEntity => new()
        {
            ProductId = Guid.NewGuid(),
            Price = _random.Next(1, 100),
            Quantity = _random.Next(1, 100),
            Title = "TEST",
        };
    }
}
