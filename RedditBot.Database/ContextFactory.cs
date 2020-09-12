using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace UltimateRedditBot.Database
{
    public class ContextFactory : IDesignTimeDbContextFactory<Context>
    {
        public Context CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                 .AddJsonFile("appsettings.json")
                 .Build();

            var dbContextBuilder = new DbContextOptionsBuilder();

            var connectionString = configuration["ConnectionString:DefaultConnection"];

            //dbContextBuilder.UseNpgsql(connectionString); postgreql con string
            dbContextBuilder.UseSqlServer(connectionString);

            return new Context(dbContextBuilder.Options);
        }
    }
}
