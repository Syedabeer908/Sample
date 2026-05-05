using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Entities
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid RefreshTokenId { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public Guid? ReplacedByTokenId { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        public bool IsRevoked { get; set; } = false;

        public User User { get; set; } = null!;
    }
}
