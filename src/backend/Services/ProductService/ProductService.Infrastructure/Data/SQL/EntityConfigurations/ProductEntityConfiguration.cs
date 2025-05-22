using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain.Entities;

namespace ProductService.Infrastructure.Data.SQL.EntityConfigurations
{
    internal class ProductEntityConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Title).HasMaxLength(64).IsRequired();
            builder.Property(p => p.Price).IsRequired();
            builder.Property(p => p.IsAvailable).IsRequired();
            builder.Property(p => p.UnitOfMeasure).IsRequired()
                .HasConversion<string>();

            builder.HasMany(p => p.Categories).WithMany(p => p.Products);
        }
    }
}
