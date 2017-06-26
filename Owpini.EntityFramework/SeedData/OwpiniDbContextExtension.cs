using Owpini.Core.Business;
using System;
using System.Collections.Generic;

namespace Owpini.EntityFramework.SeedData
{
    public static class OwpiniDbContextExtension
    {
        public static void EnsureSeedDataForContext(this OwpiniDbContext context)
        {
            context.Businesses.RemoveRange(context.Businesses);
            context.SaveChanges();

            var businesses = new List<Business>()
            {
                new Business{
                     Id = new Guid("25320c5e-f58a-4b1f-b63a-8ee07a840bdf"),
                     Name = "Chipotle"
                },
                new Business{
                    Id = new Guid("a3749477-f823-4124-aa4a-fc9ad5e79cd6"),
                     Name = "In n Out"
                }

            };

            context.Businesses.AddRange(businesses);
            context.SaveChanges();
        }
    }
}
