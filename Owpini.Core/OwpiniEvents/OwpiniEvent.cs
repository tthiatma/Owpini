using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Owpini.Core.OwpiniEvents
{
    public class OwpiniEvent
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

    }
}
