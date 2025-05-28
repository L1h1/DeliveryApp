namespace OrderService.Application.DTOs.Response
{
    public sealed record OrderResponseDTO
    {
        public Guid Id { get; init; }
        public DateTime CreatedAt { get; init; }
        public decimal TotalPrice { get; init; }
        public string Address { get; init; }
    }
}
