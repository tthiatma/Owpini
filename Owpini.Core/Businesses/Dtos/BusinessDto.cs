using Owpini.Core.Hateoas.Dtos;
using System;

namespace Owpini.Core.Businesses.Dtos
{
    public class BusinessDto : LinkedResourceBaseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }
    }
}
