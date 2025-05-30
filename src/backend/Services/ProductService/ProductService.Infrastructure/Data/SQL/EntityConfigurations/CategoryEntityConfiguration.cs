using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain.Entities;

namespace ProductService.Infrastructure.Data.SQL.EntityConfigurations
{
    internal class CategoryEntityConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.Id);

            builder.HasIndex(c => c.NormalizedName).IsUnique();

            builder.Property(c => c.NormalizedName).HasMaxLength(64).IsRequired();
            builder.Property(c => c.Name).HasMaxLength(64).IsRequired();
        }
    }
}
