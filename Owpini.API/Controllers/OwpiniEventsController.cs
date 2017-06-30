using Microsoft.AspNetCore.Mvc;
using Owpini.EntityFramework.EntityFramework.Repositories;
using AutoMapper;
using Owpini.Core.Businesses.Dtos;
using Owpini.API.Helpers;
using System.Collections.Generic;
using Newtonsoft.Json;
using Owpini.EntityFramework;
using Owpini.EntityFramework.Helpers;
using Owpini.Core.OwpiniEvents.Dtos;
using Owpini.Core.OwpiniEvents;

namespace Owpini.API.Controllers
{
    [Route("api/events")]
    public class OwpiniEventsController : Controller
    {
        private IOwpiniRepository _owpiniRepository;
        private IUrlHelper _urlHelper;
        private IPropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;

        public OwpiniEventsController(IOwpiniRepository owpiniRepository,
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
        [HttpGet(Name = "GetOwpiniEvents")]
        public IActionResult GetOwpiniEvents(CommonResourceParameters comResourceParam)
        {
            //if (!_propertyMappingService.ValidMappingExistsFor<OwpiniEventDto, OwpiniEvent>
            //   (comResourceParam.OrderBy))
            //{
            //    return BadRequest();
            //}

            //if (!_typeHelperService.TypeHasProperties<OwpiniEventDto>
            //    (comResourceParam.Fields))
            //{
            //    return BadRequest();
            //}

            var owEventsFromRepo = _owpiniRepository.GetOwpiniEvents(comResourceParam);

            var previousPageLink = owEventsFromRepo.HasPrevious ?
                ResourceUri.CreateResourceUri("GetOwpiniEvents", _urlHelper, comResourceParam, ResourceUriType.PreviousPage) : null;

            var nextPageLink = owEventsFromRepo.HasNext ?
                ResourceUri.CreateResourceUri("GetOwpiniEvents", _urlHelper, comResourceParam, ResourceUriType.NextPage) : null;

            var paginationMetaData = new
            {
                totalCount = owEventsFromRepo.TotalCount,
                pageSize = owEventsFromRepo.PageSize,
                currentPage = owEventsFromRepo.CurrentPage,
                totalPages = owEventsFromRepo.TotalPages,
                previousPageLink = previousPageLink,
                nextPageLink = nextPageLink
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetaData));

            var events = Mapper.Map<IEnumerable<OwpiniEventDto>>(owEventsFromRepo);
            return Ok(events);
        }     
    }
}
