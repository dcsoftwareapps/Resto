using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Resto.Infrastructure.Persistence;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Resto");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Set ConnectionStrings__Resto for EF Core tooling.");
        }

        var options = new DbContextOptionsBuilder().UseNpgsql(connectionString).Options;
        return new AppDbContext(options, null);
    }
}
