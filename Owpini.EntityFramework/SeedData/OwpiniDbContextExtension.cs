﻿using Owpini.Core.Business;
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
                     Description = "Mexican Restaurant"
                },
                new Business{
                    Id = new Guid("a3749477-f823-4124-aa4a-fc9ad5e79cd6"),
                     Name = "In n Out",
                     Description = "Good Burger"
                },
                new Business{
                    Id = new Guid("a5855f55-7eb6-4e9e-b1a3-f1b38dd494f4"),
                     Name = "Panda Express",
                     Description = "Get That orange chicken!"
                }
            };

            context.Businesses.AddRange(businesses);
            context.SaveChanges();
        }
    }
}
