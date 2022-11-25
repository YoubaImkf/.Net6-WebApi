using System.ComponentModel.DataAnnotations;

namespace WebApiDemo.Dtos

{  
    public class UserAddOrUpdateDto
    {
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required,EmailAddress]
        public string? Email { get; set; }
    }
}
