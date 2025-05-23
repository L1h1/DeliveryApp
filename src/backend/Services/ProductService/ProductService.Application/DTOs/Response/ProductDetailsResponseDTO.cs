using ProductService.Domain.Entities;

namespace ProductService.Application.DTOs.Response
{
    public class ProductDetailsResponseDTO
    {
        required public string ProductId { get; init; }
        public string? Description { get; init; }
        public NutritionInfo? NutritionInfo { get; init; }
        public List<string>? Composition { get; init; }
        public List<string>? Images { get; init; }
    }
}
