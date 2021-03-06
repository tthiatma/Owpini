﻿using Microsoft.AspNetCore.Mvc;
using Owpini.EntityFramework.EntityFramework.Repositories;
using AutoMapper;
using Owpini.API.Helpers;
using System.Collections.Generic;
using Newtonsoft.Json;
using Owpini.EntityFramework;
using Owpini.EntityFramework.Helpers;
using Owpini.Core.OwpiniEvents.Dtos;

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
        //[HttpGet(Name = "GetOwpiniEvents")]
        //public IActionResult GetOwpiniEvents(CommonResourceParameters comResourceParam)
        //{
        //}     
    }
}
