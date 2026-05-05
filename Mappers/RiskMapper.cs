using WebApplication1.DTOs.Risk;
using WebApplication1.Entities;

namespace WebApplication1.Mappers
{
    public class RiskMapper
    {
        public RiskDto ToDto(Risk risk)
        {
            return new RiskDto
            {
                RiskId = risk.RiskId,
                RiskTitle = risk.RiskTitle,
                RiskDescription = risk.RiskDescription
            };
        }

        public Risk ToEntity(Guid userId, CreateRiskDto dto)
        {
            return new Risk
            {
                RiskId = Guid.NewGuid(),
                UserId = userId,
                RiskTitle = dto.RiskTitle,
                RiskDescription = dto.RiskDescription,
                CreatedBy = userId
            };
        }

        public void UpdateEntity(Risk risk, Guid userId, UpdateRiskDto dto)
        {
            risk.RiskTitle = dto.RiskTitle;
            risk.RiskDescription = dto.RiskDescription;
            risk.LastUpdatedAt = DateTime.UtcNow;
            risk.LastUpdatedBy = userId;
        }

        public void PatchEntity(Risk risk, Guid userId, PatchRiskDto dto)
        {
            if (!string.IsNullOrEmpty(dto.RiskTitle))
                risk.RiskTitle = dto.RiskTitle;
            if (!string.IsNullOrEmpty(dto.RiskDescription))
                risk.RiskDescription = dto.RiskDescription;
            risk.LastUpdatedAt = DateTime.UtcNow;
            risk.LastUpdatedBy = userId;
        }

    }
}
