namespace WebApplication1.DTOs.RiskControl
{
    public class CreateRiskControlDto
    {
        public Guid RiskId { get; set; }
        public Guid ControlId { get; set; }
        public required string ControlMethod { get; set; }
    }
}
