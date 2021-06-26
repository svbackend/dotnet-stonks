using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;
using Stonks.Shared.Models;

namespace Stonks.Server.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<StockChartOhlcItem> ChartOhlcItems { get; set; }

        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }
        
        static ApplicationDbContext()
            => NpgsqlConnection.GlobalTypeMapper.MapEnum<Market>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Stock>()
                .HasAlternateKey(model => new {model.IdCompany, model.Ticker});
            
            builder.Entity<StockChartOhlcItem>()
                .HasAlternateKey(model => new {model.IdStock, model.Timestamp});
            
            builder.Entity<Company>()
                .HasIndex(model => model.Cik)
                .IsUnique();
            
            builder.Entity<Company>()
                .HasIndex(model => model.Figi)
                .IsUnique();

            builder.HasPostgresEnum<Market>();

            base.OnModelCreating(builder);
        }
    }
}
