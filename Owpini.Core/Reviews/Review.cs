using Owpini.Core.Businesses;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Owpini.Core.Reviews
{
    public class Review
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public int Star { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [ForeignKey("BusinessId")]
        public Business Business { get; set; }

        public Guid BusinessId { get; set; }
    }
}
