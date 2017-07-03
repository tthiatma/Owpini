using System;
using System.Collections.Generic;
using System.Text;

namespace Owpini.Core.Hateoas.Dtos
{
    public abstract class LinkedResourceBaseDto
    {
        public List<LinkDto> Links { get; set; }
        = new List<LinkDto>();
    }
}
