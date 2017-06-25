using Owpini.Core.Business;
using System;
using System.Collections.Generic;
using System.Text;

namespace Owpini.EntityFramework.EntityFramework.Repositories
{
    public interface IOwpiniRepository
    {
        Business GetBusiness(Guid businessId);
    }
}
