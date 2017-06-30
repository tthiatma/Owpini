using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Owpini.Core.Businesses;
using Owpini.Core.OwpiniEvents;
using Owpini.Core.OwpiniUsers;
using Owpini.Core.Reviews;

namespace Owpini.EntityFramework
{
    public class OwpiniDbContext: IdentityDbContext<OwpiniUser>
    {
        public OwpiniDbContext(DbContextOptions<OwpiniDbContext> options)
            :base(options)
        {
            Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<Business> Businesses { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<OwpiniEvent> OwpiniEvents { get; set; }
    }
}
