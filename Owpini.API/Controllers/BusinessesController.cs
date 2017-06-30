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

namespace Owpini.API.Controllers
{
    [Route("api/businesses")]
    public class BusinessesController : Controller
    {
        private IOwpiniRepository _owpiniRepository;
        private IUrlHelper _urlHelper;
        private IPropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;

        public BusinessesController(IOwpiniRepository owpiniRepository, 
            IUrlHelper urlHelper, 
            IPropertyMappingService propertyMappingService,
            ITypeHelperService typeHelperService)
        {
            _owpiniRepository = owpiniRepository;
            _urlHelper = urlHelper;
            _propertyMappingService = propertyMappingService;
            _typeHelperService = typeHelperService;
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

            var previousPageLink = businessesFromRepo.HasPrevious ?
                ResourceUri.CreateResourceUri("GetBusinesses", _urlHelper, comResourceParam, ResourceUriType.PreviousPage) : null;

            var nextPageLink = businessesFromRepo.HasNext ?
                ResourceUri.CreateResourceUri("GetBusinesses", _urlHelper, comResourceParam, ResourceUriType.NextPage) : null;

            var paginationMetaData = new
            {
                totalCount = businessesFromRepo.TotalCount,
                pageSize = businessesFromRepo.PageSize,
                currentPage = businessesFromRepo.CurrentPage,
                totalPages = businessesFromRepo.TotalPages,
                previousPageLink = previousPageLink,
                nextPageLink = nextPageLink
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetaData));

            var businesses = Mapper.Map<IEnumerable<BusinessDto>>(businessesFromRepo);
            return Ok(businesses);
        }

        [HttpGet("{id}", Name = "GetBusiness")]
        public IActionResult GetBusiness(Guid id)
        {
            var businessFromRepo = _owpiniRepository.GetBusiness(id);

            if (businessFromRepo == null)
            {
                return NotFound();
            }

            var author = Mapper.Map<BusinessDto>(businessFromRepo);
            return Ok(author);

        }

        [HttpPost]
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

        [HttpPut("{id}")]
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

        [HttpPatch("{id}")]
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
    }
}
