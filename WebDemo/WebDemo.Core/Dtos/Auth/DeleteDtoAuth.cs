using System.ComponentModel.DataAnnotations;

namespace WebDemo.Core.Dtos.Auth
{
    public class DeleteDtoAuth
    {
        [Required(ErrorMessage ="Check the Id")]
        public string? Id { get; set; }
    }
}
