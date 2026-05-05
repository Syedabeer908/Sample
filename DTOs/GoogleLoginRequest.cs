using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOs
{
    public class GoogleLoginRequest
    {
        [Required]
        public string IdToken { get; set; } = null!;
    }
}
