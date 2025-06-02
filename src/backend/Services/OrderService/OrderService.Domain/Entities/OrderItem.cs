namespace OrderService.Domain.Entities
{
    public class OrderItem
    {
        public Guid ProductId { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
