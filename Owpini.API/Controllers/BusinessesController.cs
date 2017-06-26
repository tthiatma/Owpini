using System;
using Microsoft.AspNetCore.Mvc;
using Owpini.EntityFramework.EntityFramework.Repositories;
using AutoMapper;
using Owpini.Core.Business.Dtos;

namespace Owpini.API.Controllers
{
    [Route("api/businesses")]
    public class BusinessesController : Controller
    {
        private IOwpiniRepository _owpiniRepository;
        private IUrlHelper _urlHelper;


        public BusinessesController(IOwpiniRepository owpiniRepository, IUrlHelper urlHelper)
        {
            _owpiniRepository = owpiniRepository;
            _urlHelper = urlHelper;
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
    }
}
