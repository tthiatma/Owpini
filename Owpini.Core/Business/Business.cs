using System;
using System.ComponentModel.DataAnnotations;

namespace Owpini.Core.Business
{
    public class Business
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

    }
}
