using ProductService.Domain.Enums;

namespace ProductService.Application.DTOs.Request
{
    public sealed record ProductRequestDTO
    {
        required public string Title { get; init; }
        required public decimal Price { get; init; }
        required public UnitOfMeasure UnitOfMeasure { get; init; }
        required public bool IsAvailable { get; init; }
        required public Guid ManufacturerId { get; init; }
    }
}
