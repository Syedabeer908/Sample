using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOs.Admin.V1
{
    public class CreateUserDto : AuthRegisterDto
    {
        [Required]
        public Guid RoleId { get; set; }
    }
}
