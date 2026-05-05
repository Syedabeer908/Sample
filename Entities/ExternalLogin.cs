using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Entities
{
    public class ExternalLogin : BaseEntity
    {
        [Required]
        public Guid ExternalLoginId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }

        public User User { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Provider { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        public string ProviderKey { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        public string Email { get; set; }

        [Required]
        [MaxLength(500)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string? PictureUrl { get; set; }
    }
}
