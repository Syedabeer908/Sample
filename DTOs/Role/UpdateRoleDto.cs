using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.DTOs.Role
{
    public class UpdateRoleDto
    {
        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public required string RoleName { get; set; }
    }
}
