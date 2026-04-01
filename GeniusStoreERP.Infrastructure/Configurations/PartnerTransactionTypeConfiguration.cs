using GeniusStoreERP.Domain.Entities.Finances;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeniusStoreERP.Infrastructure.Configurations;

public class PartnerTransactionTypeConfiguration : IEntityTypeConfiguration<PartnerTransactionType>
{
    public void Configure(EntityTypeBuilder<PartnerTransactionType> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasData(
            new PartnerTransactionType { Id = 1, Name = "رصيد افتتاحي" },
            new PartnerTransactionType { Id = 2, Name = "فاتورة مبيعات" },
            new PartnerTransactionType { Id = 3, Name = "فاتورة مشتريات" },
            new PartnerTransactionType { Id = 4, Name = "سند قبض" },
            new PartnerTransactionType { Id = 5, Name = "سند صرف" },
            new PartnerTransactionType { Id = 6, Name = "مرتجع مبيعات" },
            new PartnerTransactionType { Id = 7, Name = "مرتجع مشتريات" },
            new PartnerTransactionType { Id = 8, Name = "إشعار دائن" },
            new PartnerTransactionType { Id = 9, Name = "إشعار مدين" },
            new PartnerTransactionType { Id = 10, Name = "الغاء فاتورة" }
        );
    }
}
