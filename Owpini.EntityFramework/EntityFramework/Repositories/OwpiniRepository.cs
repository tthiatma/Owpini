using System;
using Owpini.Core.Businesses;
using System.Linq;
using System.Collections.Generic;
using Owpini.API.Helpers;
using Owpini.Core.Businesses.Dtos;
using Owpini.EntityFramework.Helpers;

namespace Owpini.EntityFramework.EntityFramework.Repositories
{
    public class OwpiniRepository : IOwpiniRepository
    {
        private OwpiniDbContext _context;
        private IPropertyMappingService _propertyMappingService;

        public OwpiniRepository(OwpiniDbContext context, IPropertyMappingService propertyMappingService)
        {
            _context = context;
            _propertyMappingService = propertyMappingService;
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

        public PagedList<Business> GetBusinesses(BusinessesResourceParameters businessResourceParameters)
        {
            var collectionBeforePaging =
                _context.Businesses.ApplySort(businessResourceParameters.OrderBy, _propertyMappingService.GetPropertyMapping<BusinessDto, Business>());

            if (!string.IsNullOrEmpty(businessResourceParameters.SearchQuery))
            {
                var searchQueryForWhereClause = businessResourceParameters.SearchQuery
                    .Trim().ToLowerInvariant();

                collectionBeforePaging = collectionBeforePaging
                    .Where(b => b.Name.Contains(searchQueryForWhereClause));
            }

            return PagedList<Business>.Create(collectionBeforePaging,
                businessResourceParameters.PageNumber,
                businessResourceParameters.PageSize);

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
