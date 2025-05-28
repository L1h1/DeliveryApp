using OrderService.Domain.Entities;
using OrderService.Domain.Enums;

namespace OrderService.Application.DTOs.Response
{
    public class DetailedOrderResponseDTO
    {
        public Guid Id { get; init; }
        public DateTime CreatedAt { get; init; }
        public OrderStatus OrderStatus { get; init; }
        public decimal TotalPrice { get; init; }
        public List<OrderItem> OrderItems { get; init; }
        public string Address { get; init; }
        public string? ClientComment { get; init; }
    }
}
