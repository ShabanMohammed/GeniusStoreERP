using GeniusStoreERP.Domain.Entities.Stock;
using Microsoft.EntityFrameworkCore;

namespace GeniusStoreERP.Infrastructure.Configurations;

public class StockTransactionTypeConfiguration : IEntityTypeConfiguration<StockTransactionType>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<StockTransactionType> builder)
    {
        builder.HasKey(s => s.Id);
        builder.HasIndex(s => s.Name).IsUnique();
        builder.Property(s => s.Name).HasMaxLength(50);
        builder.HasData(
           new StockTransactionType { Id = 1, Name = "فاتورة" },
           new StockTransactionType { Id = 2, Name = "تسوية" },
           new StockTransactionType { Id = 3, Name = "تلف" }
           );
    }
}
