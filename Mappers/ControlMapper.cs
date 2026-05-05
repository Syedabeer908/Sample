using WebApplication1.DTOs.Control;
using WebApplication1.Entities;

namespace WebApplication1.Mappers
{
    public class ControlMapper
    {
        public ControlDto ToDto(Control control)
        {
            return new ControlDto
            {
                ControlId = control.ControlId,
                ControlTitle = control.ControlTitle,
                ControlDescription = control.ControlDescription
            };
        }

        public Control ToEntity(Guid userId, CreateControlDto dto)
        {
            return new Control
            {
                ControlId = Guid.NewGuid(),
                UserId = userId,
                ControlTitle = dto.ControlTitle,
                ControlDescription = dto.ControlDescription,
                CreatedBy = userId
            };
        }

        public void UpdateEntity(Control control, Guid userId, UpdateControlDto dto)
        {
            control.ControlTitle = dto.ControlTitle;
            control.ControlDescription = dto.ControlDescription;
            control.LastUpdatedAt = DateTime.UtcNow;
            control.LastUpdatedBy = userId;
        }

        public void PatchEntity(Control control, Guid userId, PatchControlDto dto)
        {
            if (!string.IsNullOrEmpty(dto.ControlTitle))
                control.ControlTitle = dto.ControlTitle;
            if (!string.IsNullOrEmpty(dto.ControlDescription))
                control.ControlDescription = dto.ControlDescription;
            control.LastUpdatedAt = DateTime.UtcNow;
            control.LastUpdatedBy = userId;
        }
    }
}
