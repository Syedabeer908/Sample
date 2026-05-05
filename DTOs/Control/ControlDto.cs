namespace WebApplication1.DTOs.Control
{
    public class ControlDto
    {
        public required Guid ControlId { get; set; }
        public required string ControlTitle { get; set; }
        public required string ControlDescription { get; set; }
    }
}
