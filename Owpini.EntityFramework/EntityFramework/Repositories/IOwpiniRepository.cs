using Owpini.API.Helpers;
using Owpini.Core.Businesses;
using System;
using System.Collections.Generic;

namespace Owpini.EntityFramework.EntityFramework.Repositories
{
    public interface IOwpiniRepository
    {
        PagedList<Business> GetBusinesses(BusinessesResourceParameters businessResourceParameters);
        IEnumerable<Business> GetBusinesses();
        Business GetBusiness(Guid businessId);
        void AddBusiness(Business business);
        void UpdateBusiness(Business business);
        bool BusinessExists(Guid businessId);
        bool Save();

    }
}
