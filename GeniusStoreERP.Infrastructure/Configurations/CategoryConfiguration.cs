using GeniusStoreERP.Domain.Entities.Stock;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeniusStoreERP.Infrastructure.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasIndex(c => c.Name)
                .IsUnique();
        builder.Property(c => c.Name).IsRequired()
                .HasMaxLength(100);
        builder.Property(c => c.Description)
                .HasMaxLength(500);
    }
}
