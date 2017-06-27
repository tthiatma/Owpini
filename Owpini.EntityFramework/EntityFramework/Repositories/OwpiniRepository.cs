using System;
using Owpini.Core.Business;
using System.Linq;
using System.Collections.Generic;

namespace Owpini.EntityFramework.EntityFramework.Repositories
{
    public class OwpiniRepository : IOwpiniRepository
    {
        private OwpiniDbContext _context;

        public OwpiniRepository(OwpiniDbContext context)
        {
            _context = context;
        }

        public void AddBusiness(Business business)
        {
            business.Id = Guid.NewGuid();
            _context.Businesses.Add(business);
        }

        public bool BusinessExists(Guid businessId)
        {
            return _context.Businesses.Any(b => b.Id == businessId);
        }

        public Business GetBusiness(Guid businessId)
        {
            return _context.Businesses.FirstOrDefault(b => b.Id == businessId);
        }

        public IEnumerable<Business> GetBusinesses()
        {
            return _context.Businesses.Take(5);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

        public void UpdateBusiness(Business business)
        {
            //no code in this implementation
        }
    }
}
