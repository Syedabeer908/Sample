using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Entities
{
    public class UserLoginHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid UserLoginHistoryId { get; set; }

        [Required]
        public Guid UserId { get; set; }
        public User User { get; set; }

        [Required]
        public string IpAddress { get; set; }

        [Required]
        public string DeviceInfo { get; set; }

        public DateTime LoginTime { get; set; } = DateTime.UtcNow;
    }
}
