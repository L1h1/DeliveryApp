namespace ProductService.Domain.Entities
{
    public class Category
    {
        required public Guid Id { get; set; }
        required public string Name { get; set; }
        required public string NormalizedName { get; set; }
        public List<Product> Products { get; set; } = new ();
    }
}
