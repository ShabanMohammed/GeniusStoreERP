using GeniusStoreERP.Domain.Entities.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeniusStoreERP.Infrastructure.Configurations;

public class InvoiceTypeConfiguration : IEntityTypeConfiguration<InvoiceType>
{
    public void Configure(EntityTypeBuilder<InvoiceType> builder)
    {
        builder.HasKey(it => it.Id);
        builder.HasIndex(it => it.Name)
        .IsUnique();

        builder.Property(it => it.Name)
               .IsRequired()
               .HasMaxLength(50);

        builder.HasMany(it => it.Invoices)
               .WithOne(i => i.InvoiceType)
               .HasForeignKey(i => i.InvoiceTypeId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
           new InvoiceType { Id = 1, Name = "مبيعات" },
           new InvoiceType { Id = 2, Name = "مشتريات" },
           new InvoiceType { Id = 3, Name = "مرتجع مبيعات" },
           new InvoiceType { Id = 4, Name = "مرتجع مشتريات" }
       );

    }
}
