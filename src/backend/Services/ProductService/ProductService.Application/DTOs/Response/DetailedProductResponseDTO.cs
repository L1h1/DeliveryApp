using ProductService.Domain.Entities;

namespace ProductService.Application.DTOs.Response
{
    public sealed record DetailedProductResponseDTO
    {
        public Guid Id { get; init; }
        public string Title { get; init; }
        public decimal Price { get; init; }
        public string UnitOfMeasure { get; init; }
        public string Description { get; init; }
        public NutritionInfo? Nutrition { get; set; }
        public List<string> Composition { get; init; }
        public List<string>? Images { get; set; }
        public string ManufacturerName { get; init; }
        public string ManufacturerCountry { get; init; }
        public string ManufacturerAddress { get; init; }
    }
}
