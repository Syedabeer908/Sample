namespace WebApplication1.DTOs
{
    public class BaseUserDto
    {
        public required Guid UserId { get; set; } 
        public required string Username { get; set; }
        public required string Email { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
}
