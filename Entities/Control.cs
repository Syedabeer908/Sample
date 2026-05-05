using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Entities
{
    public class Control : BaseEntity
    {
        [Required]
        public Guid ControlId { get; set; }

        [Required]
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        [Required]
        [Column(TypeName = "nvarchar(200)")]
        public string ControlTitle { get; set; }

        [Column(TypeName = "nvarchar(1000)")]
        public string ControlDescription { get; set; }

        public ICollection<RiskControl> RiskControls { get; set; } = new List<RiskControl>();
    }
}
