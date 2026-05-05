namespace WebApplication1.DTOs
{
    public class RedisUserDto
    {
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int TokenVersion { get; set; }
    }
}
