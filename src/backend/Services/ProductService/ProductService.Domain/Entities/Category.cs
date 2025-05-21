namespace ProductService.Domain.Entities
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }

        public List<Product> Products { get; set; } = new ();
    }
}
