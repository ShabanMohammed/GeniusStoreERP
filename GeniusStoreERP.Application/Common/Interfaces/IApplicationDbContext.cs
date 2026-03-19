using GeniusStoreERP.Domain.Entities;
using GeniusStoreERP.Domain.Entities.Stock;
using Microsoft.EntityFrameworkCore;

namespace GeniusStoreERP.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    }
}
