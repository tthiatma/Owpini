using System;
using System.Collections.Generic;
using System.Text;
using Owpini.Core.Business;
using System.Linq;

namespace Owpini.EntityFramework.EntityFramework.Repositories
{
    public class OwpiniRepository : IOwpiniRepository
    {
        private OwpiniDbContext _context;

        public OwpiniRepository(OwpiniDbContext context)
        {
            _context = context;
        }
            
        public Business GetBusiness(Guid businessId)
        {
            return _context.Businesses.FirstOrDefault(b => b.Id == businessId);
        }
    }
}
