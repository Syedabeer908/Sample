using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOs
{
    public class RefreshTokenDto
    {
        [Required]
        public required string Token { get; set; }
    }
}
