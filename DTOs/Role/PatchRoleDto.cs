using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.DTOs.Role
{
    public class PatchRoleDto
    {
        [Column(TypeName = "nvarchar(100)")]
        public string? RoleName { get; set; }
    }
}
