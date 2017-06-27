using Owpini.Core.Business;
using System;
using System.Collections.Generic;

namespace Owpini.EntityFramework.EntityFramework.Repositories
{
    public interface IOwpiniRepository
    {
        IEnumerable<Business> GetBusinesses();
        Business GetBusiness(Guid businessId);
        void AddBusiness(Business business);
        void UpdateBusiness(Business business);
        bool BusinessExists(Guid businessId);
        bool Save();

    }
}
