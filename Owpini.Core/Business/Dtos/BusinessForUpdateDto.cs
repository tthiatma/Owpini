using System.ComponentModel.DataAnnotations;

namespace Owpini.Core.Business.Dtos
{
    public class BusinessForUpdateDto : BusinessForManipulationDto
    {
        [Required(ErrorMessage = "You should fill out a description.")]

        public override string Description
        {
            get => base.Description;
            set => base.Description = value;
        }
    }
}
