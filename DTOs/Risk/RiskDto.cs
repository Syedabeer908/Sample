namespace WebApplication1.DTOs.Risk
{
    public class RiskDto
    {
        public required Guid RiskId { get; set; }
        public required string RiskTitle { get; set; }
        public required string RiskDescription { get; set; }
    }
}