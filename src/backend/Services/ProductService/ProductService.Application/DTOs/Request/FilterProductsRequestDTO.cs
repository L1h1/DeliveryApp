namespace ProductService.Application.DTOs.Request
{
    public sealed record FilterProductsRequestDTO
    {
        public string? SearchTerm { get; init; }
        public List<Guid>? CategoryIds { get; init; }
        required public PageRequestDTO Page { get; init; }
    }
}
