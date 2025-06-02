namespace OrderService.Application.DTOs.Response
{
    public class ProductResponseDTO
    {
        public Guid Id { get; init; }
        public string Title { get; init; }
        public decimal Price { get; init; }
    }
}
