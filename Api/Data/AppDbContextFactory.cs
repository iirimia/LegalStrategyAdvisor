using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using LegalStrategyAdvisor.Api.Data;

namespace LegalStrategyAdvisor.Api.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            // Default connection string for migrations
            var connectionString = "Host=localhost;Port=5432;Database=legal_strategy;Username=postgres;Password=postgres";

            optionsBuilder.UseNpgsql(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
