using ProductService.Domain.Entities;

namespace ProductService.Application.DTOs.Response
{
    public sealed record DetailedProductResponseDTO
    {
        required public Guid Id { get; init; }
        required public string Title { get; init; }
        required public decimal Price { get; init; }
        required public string UnitOfMeasure { get; init; }
        required public string ManufacturerName { get; init; }
        required public string ManufacturerCountry { get; init; }
        required public string ManufacturerAddress { get; init; }
        public string? Description { get; init; }
        public NutritionInfo? Nutrition { get; set; }
        public List<string>? Composition { get; init; }
        public string? Thumbnail { get; init; }
        public List<string>? Images { get; set; }
    }
}
