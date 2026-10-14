using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EcoTracker.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<EcoTrackerDbContext>
{
    public EcoTrackerDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<EcoTrackerDbContext>();
        optionsBuilder.UseSqlServer("Server=localhost;Database=EcoTrackerDb;Trusted_Connection=True;TrustServerCertificate=True;");
        return new EcoTrackerDbContext(optionsBuilder.Options);
    }
}
