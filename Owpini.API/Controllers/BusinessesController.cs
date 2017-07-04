using System;
using Microsoft.AspNetCore.Mvc;
using Owpini.EntityFramework.EntityFramework.Repositories;
using AutoMapper;
using Owpini.Core.Businesses.Dtos;
using Owpini.Core.Businesses;
using Owpini.API.Helpers;
using Microsoft.AspNetCore.JsonPatch;
using System.Collections.Generic;
using Newtonsoft.Json;
using Owpini.EntityFramework;
using Owpini.EntityFramework.Helpers;
using System.Linq;
using Owpini.Core.Hateoas.Dtos;
using Microsoft.Extensions.Logging;

namespace Owpini.API.Controllers
{
    [Route("api/businesses")]
    public class BusinessesController : Controller
    {
        private IOwpiniRepository _owpiniRepository;
        private IUrlHelper _urlHelper;
        private IPropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;
        private ILogger<BusinessesController> _logger;

        public BusinessesController(IOwpiniRepository owpiniRepository, 
            IUrlHelper urlHelper, 
            IPropertyMappingService propertyMappingService,
            ITypeHelperService typeHelperService,
            ILogger<BusinessesController> logger)
        {
            _owpiniRepository = owpiniRepository;
            _urlHelper = urlHelper;
            _propertyMappingService = propertyMappingService;
            _typeHelperService = typeHelperService;
            _logger = logger;
        }

        // GET api/values
        [HttpGet(Name = "GetBusinesses")]
        public IActionResult GetBusinesses(CommonResourceParameters comResourceParam)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<BusinessDto, Business>
               (comResourceParam.OrderBy))
            {
                return BadRequest();
            }

            if (!_typeHelperService.TypeHasProperties<BusinessDto>
                (comResourceParam.Fields))
            {
                return BadRequest();
            }

            var businessesFromRepo = _owpiniRepository.GetBusinesses(comResourceParam);

            var businesses = Mapper.Map<IEnumerable<BusinessDto>>(businessesFromRepo);

            //var previousPageLink = businessesFromRepo.HasPrevious ?
            //    CreateBusinessResourceUri("GetBusinesses", _urlHelper, comResourceParam, ResourceUriType.PreviousPage) : null;

            //var nextPageLink = businessesFromRepo.HasNext ?
            //    CreateBusinessResourceUri("GetBusinesses", _urlHelper, comResourceParam, ResourceUriType.NextPage) : null;

            var paginationMetaData = new
            {
                totalCount = businessesFromRepo.TotalCount,
                pageSize = businessesFromRepo.PageSize,
                currentPage = businessesFromRepo.CurrentPage,
                totalPages = businessesFromRepo.TotalPages
                //previousPageLink = previousPageLink,
                //nextPageLink = nextPageLink
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetaData));

            var links = CreateLinksForBusinesses(comResourceParam,
                businessesFromRepo.HasNext, businessesFromRepo.HasPrevious);

            var shapedBusinesses = businesses.ShapeData(comResourceParam.Fields);

            var shapedBusinessesWithLinks = shapedBusinesses.Select(biz =>
            {
            var bizAsDictionary = biz as IDictionary<string, object>;
                var businessLinks = CreateLinksForBusiness(
                    (Guid)bizAsDictionary["Id"], comResourceParam.Fields);

                bizAsDictionary.Add("links", businessLinks);

                return bizAsDictionary;
            });

            var linkedCollectionResource = new
            {
                value = shapedBusinessesWithLinks,
                links = links
            };

            return Ok(linkedCollectionResource);
        }

        [HttpGet("{id}", Name = "GetBusiness")]
        public IActionResult GetBusiness(Guid id, [FromQuery] string fields)
        {
            if (!_typeHelperService.TypeHasProperties<BusinessDto>(fields))
            {
                return BadRequest();
            }

            var businessFromRepo = _owpiniRepository.GetBusiness(id);

            if (businessFromRepo == null)
            {
                return NotFound();
            }

            var business = Mapper.Map<BusinessDto>(businessFromRepo);

            var links = CreateLinksForBusiness(id, fields);

            var linkedResourceToReturn = business.ShapeData(fields)
                as IDictionary<string, object>;

            linkedResourceToReturn.Add("links", links);

            return Ok(linkedResourceToReturn);
        }

        [HttpDelete("{id}", Name = "DeleteBusiness")]
        public IActionResult DeleteBusiness(Guid id)
        {
            if (!_owpiniRepository.BusinessExists(id))
            {
                return NotFound();
            }

            var bizFromRepo = _owpiniRepository.GetBusiness(id);
            if (bizFromRepo == null)
            {
                return NotFound();
            }

            _owpiniRepository.DeleteBusiness(bizFromRepo);

            if (!_owpiniRepository.Save())
            {
                throw new Exception($"Deleting business {id} failed on save.");
            }

            _logger.LogInformation(100, $"business {id} was deleted.");

            return NoContent();
        }

        [HttpPost(Name ="CreateBusiness")]
        public IActionResult CreateBusiness([FromBody] BusinessForCreationDto business)
        {
            if (business == null)
            {
                return BadRequest();
            }

            var businessEntity = Mapper.Map<Business>(business);

            _owpiniRepository.AddBusiness(businessEntity);

            if (!_owpiniRepository.Save())
            {
                throw new Exception("Creating a business failed on save.");
            }

            var businessToReturn = Mapper.Map<BusinessDto>(businessEntity);

            return CreatedAtRoute("GetBusiness",
                new { id = businessToReturn.Id },
                businessToReturn);
        }

        [HttpPut("{id}", Name = "UpdateBusiness")]
        public IActionResult UpdateBusiness(Guid id, [FromBody] BusinessForUpdateDto business)
        {
            if (business == null)
            {
                return BadRequest();
            }

            if (business.Name == business.Description)
            {
                ModelState.AddModelError(nameof(BusinessForUpdateDto), "The provided description should be different from title");
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var businessFromRepo = _owpiniRepository.GetBusiness(id);

            if (businessFromRepo == null)
            {
                var businessToAdd = Mapper.Map<Business>(business);
                businessToAdd.Id = id;

                _owpiniRepository.AddBusiness(businessToAdd);

                if (!_owpiniRepository.Save())
                {
                    throw new Exception($"Upserting business {id} failed on save");
                }

                var businessToReturn = Mapper.Map<BusinessDto>(businessToAdd);

                return CreatedAtRoute("GetBusiness", new { id = businessToReturn.Id }, businessToReturn);

            }

            Mapper.Map(business, businessFromRepo);

            _owpiniRepository.UpdateBusiness(businessFromRepo);

            if (!_owpiniRepository.Save())
            {
                throw new Exception($"Updating business {id} failed on save");
            }

            return NoContent();
        }

        [HttpPatch("{id}", Name = "PartiallyUpdateBusiness")]
        public IActionResult PartiallyUpdateBusiness(Guid id, 
            [FromBody] JsonPatchDocument<BusinessForUpdateDto> patchDoc)
        {
            if(patchDoc == null)
            {
                return BadRequest();
            }

            var businessFromRepo = _owpiniRepository.GetBusiness(id);

            if (businessFromRepo == null)
            {
                var businessDto = new BusinessForUpdateDto();

                patchDoc.ApplyTo(businessDto, ModelState);

                if(businessDto.Description == businessDto.Name)
                {
                    ModelState.AddModelError(nameof(BusinessForUpdateDto),
                        "The provided description should be different from the name");
                }

                TryValidateModel(businessDto);

                if(!ModelState.IsValid)
                {
                    return new UnprocessableEntityObjectResult(ModelState);
                }
                var businessToAdd = Mapper.Map<Business>(businessDto);
                businessToAdd.Id = id;

                _owpiniRepository.AddBusiness(businessToAdd);

                if (!_owpiniRepository.Save())
                {
                    throw new Exception($"Upserting business {id} failed on save");
                }

                var businessToReturn = Mapper.Map<BusinessDto>(businessToAdd);
                return CreatedAtRoute("GetBusiness", new { id = businessToReturn.Id }, businessToReturn);
            }

            var businessToPatch = Mapper.Map<BusinessForUpdateDto>(businessFromRepo);

            patchDoc.ApplyTo(businessToPatch, ModelState);

            if(businessToPatch.Description == businessToPatch.Name)
            {
                ModelState.AddModelError(nameof(BusinessForUpdateDto),
                    "The provided description should be different than the name");
            }

            TryValidateModel(businessToPatch);

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            Mapper.Map(businessToPatch, businessFromRepo);

            _owpiniRepository.UpdateBusiness(businessFromRepo);

            if(!_owpiniRepository.Save())
            {
                throw new Exception($"Patching business {id} failed on save");
            }

            return NoContent();
        }

        private IEnumerable<LinkDto> CreateLinksForBusinesses(
            CommonResourceParameters comResourceParameters,
            bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            // self 
            links.Add(
               new LinkDto(CreateBusinessResourceUri(comResourceParameters,
               ResourceUriType.Current), 
               "self", "GET"));

            if (hasNext)
            {
                links.Add(
                  new LinkDto(CreateBusinessResourceUri(comResourceParameters,
                  ResourceUriType.NextPage),
                  "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(
                    new LinkDto(CreateBusinessResourceUri(comResourceParameters,
                    ResourceUriType.PreviousPage),
                    "previousPage", "GET"));
            }

            return links;
        }
        private IEnumerable<LinkDto> CreateLinksForBusiness(Guid id, string fields)
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                  new LinkDto(_urlHelper.Link("GetBusiness", new { id = id }),
                  "self",
                  "GET"));
            }
            else
            {
                links.Add(
                  new LinkDto(_urlHelper.Link("GetBusiness", new { id = id, fields = fields }),
                  "self",
                  "GET"));
            }

            links.Add(
              new LinkDto(_urlHelper.Link("DeleteBusiness", new { id = id }),
              "delete_business",
              "DELETE"));

            links.Add(
             new LinkDto(_urlHelper.Link("UpdateBusiness", new { id = id }),
             "update_business",
             "UPDATE"));

            return links;
        }

        private string CreateBusinessResourceUri(
            CommonResourceParameters commonResourceParameters,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _urlHelper.Link("GetBusinesses",
                      new
                      {
                          fields = commonResourceParameters.Fields,
                          orderBy = commonResourceParameters.OrderBy,
                          searchQuery = commonResourceParameters.SearchQuery,
                          pageNumber = commonResourceParameters.PageNumber - 1,
                          pageSize = commonResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return _urlHelper.Link("GetBusinesses",
                      new
                      {
                          fields = commonResourceParameters.Fields,
                          orderBy = commonResourceParameters.OrderBy,
                          searchQuery = commonResourceParameters.SearchQuery,
                          pageNumber = commonResourceParameters.PageNumber + 1,
                          pageSize = commonResourceParameters.PageSize
                      });
                case ResourceUriType.Current:
                default:
                    return _urlHelper.Link("GetBusinesses",
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
