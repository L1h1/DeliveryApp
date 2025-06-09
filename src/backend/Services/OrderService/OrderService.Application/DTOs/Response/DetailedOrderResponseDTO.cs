using OrderService.Domain.Entities;

namespace OrderService.Application.DTOs.Response
{
    public class DetailedOrderResponseDTO
    {
        public Guid Id { get; init; }
        public DateTime CreatedAt { get; init; }
        public string OrderStatus { get; init; }
        public decimal TotalPrice { get; init; }
        public List<OrderItem> Items { get; init; }
        public string Address { get; init; }
        public string? ClientComment { get; init; }
    }
}
