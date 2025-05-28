using OrderService.Domain.Enums;

namespace OrderService.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public Guid? CourierId { get; set; }
        public DateTime CreatedAt { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public decimal TotalPrice { get; set; }
        public List<OrderItem> Items { get; set; }
        public string? ClientComment { get; set; }
    }
}
