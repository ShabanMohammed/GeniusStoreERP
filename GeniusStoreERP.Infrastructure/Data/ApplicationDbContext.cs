using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Domain.Entities;
using GeniusStoreERP.Domain.Entities.Partners;
using GeniusStoreERP.Domain.Entities.Stock;
using GeniusStoreERP.Domain.Entities.Transactions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace GeniusStoreERP.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Partner> Partners { get; set; }

    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceItem> InvoiceItems { get; set; }
    public DbSet<InvoiceType> InvoiceTypes { get; set; }
    public DbSet<InvoiceStatus> InvoiceStatuses { get; set; }


    public new DbSet<ApplicationUser> Users { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Seed Data لجدول أنواع الفواتير
        builder.Entity<InvoiceType>().HasData(
            new InvoiceType { Id = 1, Name = "مبيعات" },
            new InvoiceType { Id = 2, Name = "مشتريات" },
            new InvoiceType { Id = 3, Name = "مرتجع مبيعات" },
            new InvoiceType { Id = 4, Name = "مرتجع مشتريات" }
        );

        // Seed Data لجدول طرق السداد
        builder.Entity<InvoiceStatus>().HasData(
            new InvoiceStatus { Id = 1, Name = "نقدي" },
            new InvoiceStatus { Id = 2, Name = "آجل" }

        );

    }

}
