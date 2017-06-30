using Microsoft.AspNetCore.Mvc;
using Owpini.EntityFramework.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Owpini.API.Helpers
{
    public static class ResourceUri
    {
        public static string CreateResourceUri(
            string getUrl,
            IUrlHelper urlHelper,
            CommonResourceParameters commonResourceParameters,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return urlHelper.Link(getUrl,
                      new
                      {
                          orderBy = commonResourceParameters.OrderBy,
                          searchQuery = commonResourceParameters.SearchQuery,
                          pageNumber = commonResourceParameters.PageNumber - 1,
                          pageSize = commonResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return urlHelper.Link(getUrl,
                      new
                      {
                          orderBy = commonResourceParameters.OrderBy,
                          searchQuery = commonResourceParameters.SearchQuery,
                          pageNumber = commonResourceParameters.PageNumber + 1,
                          pageSize = commonResourceParameters.PageSize
                      });

                default:
                    return urlHelper.Link(getUrl,
                    new
                    {
                        orderBy = commonResourceParameters.OrderBy,
                        searchQuery = commonResourceParameters.SearchQuery,
                        pageNumber = commonResourceParameters.PageNumber,
                        pageSize = commonResourceParameters.PageSize
                    });
            }
        }
    }
}
