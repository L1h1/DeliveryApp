namespace ProductService.Domain.Entities
{
    public class ProductDetails
    {
        public Guid Id { get; set; }
        public string? Description { get; set; }
        public NutritionInfo? Nutrition { get; set; }
        public List<string>? Composition { get; set; }
        public List<string>? Images { get; set; }
    }
}
