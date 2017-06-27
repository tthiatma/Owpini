using Owpini.Core.Business;
using System;

namespace Owpini.EntityFramework.EntityFramework.Repositories
{
    public interface IOwpiniRepository
    {
        Business GetBusiness(Guid businessId);
        void AddBusiness(Business business);
        void UpdateBusiness(Business business);
        bool BusinessExists(Guid businessId);
        bool Save();

    }
}
