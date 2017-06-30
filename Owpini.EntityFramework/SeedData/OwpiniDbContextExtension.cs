using Owpini.Core.Businesses;
using Owpini.Core.OwpiniEvents;
using System;
using System.Collections.Generic;

namespace Owpini.EntityFramework.SeedData
{
    public static class OwpiniDbContextExtension
    {
        public static void EnsureSeedDataForContext(this OwpiniDbContext context)
        {
            context.Businesses.RemoveRange(context.Businesses);
            context.OwpiniEvents.RemoveRange(context.OwpiniEvents);

            context.SaveChanges();

            var events = new List<OwpiniEvent>()
            {
                new OwpiniEvent{
                    Id = new Guid("11f0e117-155e-461c-80d4-1292bc6d5ac0"),
                    Description = "d super duper long marathon",
                    Name = "a GOGO Marathon"
                },
                new OwpiniEvent{
                    Id = new Guid("23d2c3d4-9f90-4e86-ae2c-d27d25e6a679"),
                    Description = "e nice hawaii trip",
                    Name = "b Green Trip"
                },
                new OwpiniEvent{
                    Id = new Guid("5b74edd4-8645-4211-92d9-16ae01211b78"),
                    Description = "f boring meetup ever",
                    Name = "c failed meetup"
                },
            };

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

            context.OwpiniEvents.AddRange(events);
            context.Businesses.AddRange(businesses);
            context.SaveChanges();
        }
    }
}
