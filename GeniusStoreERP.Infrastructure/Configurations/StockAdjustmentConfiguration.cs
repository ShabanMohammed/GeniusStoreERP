using GeniusStoreERP.Domain.Entities.Stock;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeniusStoreERP.Infrastructure.Configurations;

public class StockAdjustmentConfiguration : IEntityTypeConfiguration<StockAdjustment>
{
    public void Configure(EntityTypeBuilder<StockAdjustment> builder)
    {
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.ReferenceNumber)
            .HasMaxLength(50)
            .IsRequired();
            
        builder.Property(s => s.Remarks)
            .HasMaxLength(500);

        builder.HasMany(s => s.Items)
            .WithOne(i => i.StockAdjustment)
            .HasForeignKey(i => i.StockAdjustmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.StockTransactions)
            .WithOne(t => t.StockAdjustment)
            .HasForeignKey(t => t.AdjustmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
