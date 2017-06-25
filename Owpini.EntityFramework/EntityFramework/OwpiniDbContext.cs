using Microsoft.EntityFrameworkCore;
using Owpini.Core.Business;

namespace Owpini.EntityFramework
{
    public class OwpiniDbContext: DbContext
    {
        public OwpiniDbContext(DbContextOptions<OwpiniDbContext> options)
            :base(options)
        {
        }
        public DbSet<Business> Businesses { get; set; }
    }
}
