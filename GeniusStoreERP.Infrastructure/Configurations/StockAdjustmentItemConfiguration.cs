using GeniusStoreERP.Domain.Entities.Stock;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeniusStoreERP.Infrastructure.Configurations;

public class StockAdjustmentItemConfiguration : IEntityTypeConfiguration<StockAdjustmentItem>
{
    public void Configure(EntityTypeBuilder<StockAdjustmentItem> builder)
    {
        builder.HasKey(s => s.Id);

        builder.HasOne(s => s.StockAdjustment)
            .WithMany(a => a.Items)
            .HasForeignKey(s => s.StockAdjustmentId);

        builder.HasOne(s => s.Product)
            .WithMany()
            .HasForeignKey(s => s.ProductId);

        builder.HasOne(s => s.StockTransactionType)
            .WithMany()
            .HasForeignKey(s => s.StockTransactionTypeId);
    }
}
