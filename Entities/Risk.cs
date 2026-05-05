using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Entities
{
    public class Risk : BaseEntity
    {
        [Required]
        public Guid RiskId { get; set; }

        [Required]
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        [Required]
        [Column(TypeName = "nvarchar(200)")]
        public string RiskTitle { get; set; }

        [Column(TypeName = "nvarchar(1000)")]
        public string RiskDescription { get; set; }

        public ICollection<RiskControl> RiskControls { get; set; } = new List<RiskControl>();
    }
}
