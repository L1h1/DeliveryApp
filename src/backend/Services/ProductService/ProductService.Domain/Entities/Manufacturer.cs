namespace ProductService.Domain.Entities
{
    public class Manufacturer
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public List<Product> Products { get; set; }
    }
}
