using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Entities
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; } = null;
        public DateTime? LastUpdatedAt { get; set; } = null;
        public Guid? LastUpdatedBy { get; set; } = null;
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; } = null;
        public Guid? DeletedBy { get; set; } = null;
    }
}
