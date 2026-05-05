namespace WebApplication1.DTOs.Admin.V2
{
    public class UserDto : BaseUserDto
    {
        public bool IsActive { get; set; } = true;
    }
}
