using GeniusStoreERP.Domain.Entities.Stock;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeniusStoreERP.Infrastructure.Configurations
{
       public class StockTransactionConfiguration : IEntityTypeConfiguration<StockTransaction>
       {
              public void Configure(EntityTypeBuilder<StockTransaction> builder)
              {
                     builder.HasKey(t => t.Id);
                     builder.HasIndex(t => t.ProductId);
                     builder.HasIndex(t => t.TransactionDate);
                     builder.Property(t => t.TransactionDate)
                            .IsRequired();
                     builder.Property(t => t.Quantity)
                            .HasPrecision(18, 4)
                            .IsRequired();

                     builder.Property(t => t.InvoiceId)
                            .IsRequired(false);
                     builder.Property(t => t.InvoiceReference)
                            .IsRequired(false)
                            .HasMaxLength(50);
                     builder.Property(t => t.AdjustmentId)
                            .IsRequired(false);
                     builder.Property(t => t.AdjustmentReference)
                             .IsRequired(false)
                             .HasMaxLength(50);
                     builder.Property(t => t.Remarks)
                            .HasMaxLength(500);
                     builder.HasOne(t => t.Product)
                         .WithMany(p => p.StockTransactions)
                         .HasForeignKey(t => t.ProductId)
                         .IsRequired()
                         .OnDelete(DeleteBehavior.Restrict);
                     builder.HasOne(t => t.Invoice)
                         .WithMany(i => i.StockTransactions)
                         .HasForeignKey(t => t.InvoiceId)
                         .IsRequired(false)
                         .OnDelete(DeleteBehavior.Restrict);
                     builder.HasOne(t => t.Type)
                         .WithMany(tt => tt.StockTransactions)
                         .HasForeignKey(t => t.StockTransactionTypeId)
                         .IsRequired()
                         .OnDelete(DeleteBehavior.Restrict);
              }
       }
}
