using Owpini.Core.Businesses;
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
                     Name = "Chipotle",
                     Description = "Mexican Restaurant",
                     Address1 = "123 address",
                     City = "Chandler",
                     Phone = "480-123-4567",
                     State = "AZ",
                     WebAddress = "www.chipotle.com",
                     Zip = 85286
                     

                },
                new Business{
                    Id = new Guid("a3749477-f823-4124-aa4a-fc9ad5e79cd6"),
                     Name = "In n Out",
                     Description = "Good Burger",
                     Address1 = "456 address",
                     City = "Phoenix",
                     Phone = "480-986-3457",
                     State = "AZ",
                     WebAddress = "www.innout.com",
                     Zip = 85286
                },
                new Business{
                    Id = new Guid("a5855f55-7eb6-4e9e-b1a3-f1b38dd494f4"),
                     Name = "Panda Express",
                     Description = "Get That orange chicken!",
                      Address1 = "789 address",
                     City = "Mesa",
                     Phone = "480-789-4567",
                     State = "AZ",
                     WebAddress = "www.pandaexpress.com",
                     Zip = 85286
                }
            };

            context.Businesses.AddRange(businesses);
            context.SaveChanges();
        }
    }
}
