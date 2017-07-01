using Microsoft.AspNetCore.Mvc;
using Owpini.EntityFramework.Helpers;

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
                          fields = commonResourceParameters.Fields,
                          orderBy = commonResourceParameters.OrderBy,
                          searchQuery = commonResourceParameters.SearchQuery,
                          pageNumber = commonResourceParameters.PageNumber - 1,
                          pageSize = commonResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return urlHelper.Link(getUrl,
                      new
                      {
                          fields = commonResourceParameters.Fields,
                          orderBy = commonResourceParameters.OrderBy,
                          searchQuery = commonResourceParameters.SearchQuery,
                          pageNumber = commonResourceParameters.PageNumber + 1,
                          pageSize = commonResourceParameters.PageSize
                      });

                default:
                    return urlHelper.Link(getUrl,
                    new
                    {
                        fields = commonResourceParameters.Fields,
                        orderBy = commonResourceParameters.OrderBy,
                        searchQuery = commonResourceParameters.SearchQuery,
                        pageNumber = commonResourceParameters.PageNumber,
                        pageSize = commonResourceParameters.PageSize
                    });
            }
        }
    }
}
