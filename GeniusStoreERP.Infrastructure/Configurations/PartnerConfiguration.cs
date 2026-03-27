using GeniusStoreERP.Domain.Entities.Partners;
using Microsoft.EntityFrameworkCore;

namespace GeniusStoreERP.Infrastructure.Configurations;

public class PartnerConfiguration : IEntityTypeConfiguration<Partner>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Partner> builder)
    {
        builder.HasKey(p => p.Id);
        builder.HasIndex(p => p.Name)
               .IsUnique();
        builder.HasIndex(p => p.Email)
               .IsUnique()
               .HasFilter("\"Email\" IS NOT NULL AND \"Email\" != ''");
        builder.Property(p => p.Name)
               .IsRequired().HasMaxLength(100);
        builder.Property(p => p.Email)
               .HasMaxLength(100);
        builder.Property(p => p.PhoneNumber)
               .HasMaxLength(20).IsUnicode(false);
        builder.Property(p => p.Address)
               .HasMaxLength(200);
    }
}
