using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOs
{
    public class BasePatchUserDto
    {
        [MaxLength(200)]
        public string? Username { get; set; }

        [MaxLength(100)]
        [EmailAddress]
        public string? Email { get; set; }


        [MaxLength(30)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Password must be at least 8 characters long and include at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string? Password { get; set; }
    }
}
