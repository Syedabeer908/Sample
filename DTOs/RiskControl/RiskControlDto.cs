namespace WebApplication1.DTOs.RiskControl
{
    public class RiskControlDto
    {
        public required Guid RiskControlId { get; set; }
        public required string RiskTitle { get; set; }
        public required string ControlTitle { get; set; }
        public required string ControlMethod { get; set; }
    }
}
