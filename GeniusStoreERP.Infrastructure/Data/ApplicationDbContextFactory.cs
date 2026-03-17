using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GeniusStoreERP.Infrastructure.Data;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        
        // Hardcoded connection string for design-time tools
        builder.UseSqlite("Data Source=GeniusStore.db");

        return new ApplicationDbContext(builder.Options);
    }
}
