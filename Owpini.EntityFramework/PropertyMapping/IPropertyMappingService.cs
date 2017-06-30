using System;
using System.Collections.Generic;
using System.Text;

namespace Owpini.EntityFramework
{
    public interface IPropertyMappingService
    {
        bool ValidMappingExistsFor<TSource, TDestination>(string fields);

        Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>();

    }
}
