using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOs
{
    public class CreateProfileImageDto
    {
        [Required]
        public IFormFile File { get; set; }
    }
}
