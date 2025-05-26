namespace ProductService.Application.DTOs.Response
{
    public sealed record ProductResponseDTO
    {
        required public Guid Id { get; init; }
        required public string Title { get; init; }
        required public decimal Price { get; init; }
        required public string UnitOfMeasure { get; init; }
        required public string Country { get; init; }
        required public List<string> CategoryNames { get; init; }
    }
}
