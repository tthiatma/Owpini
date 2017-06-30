using Owpini.Core.Reviews;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Owpini.Core.Businesses
{
    public class Business
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        [MaxLength(500)]
        public string Address1 { get; set; }

        public string Address2 { get; set; }

        [Required]
        [MaxLength(500)]
        public string City { get; set; }

        [Required]
        [MaxLength(500)]
        public string State { get; set; }

        [Required]
        public int Zip { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        public string WebAddress { get; set; }

        public ICollection<Review> Reviews { get; set; }
        = new List<Review>();
    }
}
