using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stonks.Shared.Models;

namespace Stonks.Server.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Company> Companies { get; set; }

        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Stock>()
                .HasKey(model => new {model.IdCompany, model.Ticker});

            base.OnModelCreating(builder);
        }
    }
}
