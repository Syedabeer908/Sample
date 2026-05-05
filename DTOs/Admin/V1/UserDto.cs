namespace WebApplication1.DTOs.Admin.V1
{
    public class UserDto : BaseUserDto
    {
        public required Guid RoleId { get; set; }

        public required string RoleName { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
