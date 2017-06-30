using Owpini.Core.Businesses;
using Owpini.Core.OwpiniEvents;
using Owpini.EntityFramework.Helpers;
using System;
using System.Collections.Generic;

namespace Owpini.EntityFramework.EntityFramework.Repositories
{
    public interface IOwpiniRepository
    {
        PagedList<Business> GetBusinesses(CommonResourceParameters commonResourceParameters);
        PagedList<OwpiniEvent> GetOwpiniEvents(CommonResourceParameters commonResourceParameters);

        IEnumerable<Business> GetBusinesses();
        Business GetBusiness(Guid businessId);
        void AddBusiness(Business business);
        void UpdateBusiness(Business business);
        bool BusinessExists(Guid businessId);
        bool Save();

    }
}
