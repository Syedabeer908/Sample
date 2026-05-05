using WebApplication1.Common.Exceptions;
using WebApplication1.DTOs.RiskControl;
using WebApplication1.Entities;
using WebApplication1.Entities.Enums;

namespace WebApplication1.Mappers
{
    public class RiskControlMapper
    {
        public RiskControlDto ToDto(RiskControl riskControl)
        {
            return new RiskControlDto
            {
                RiskControlId = riskControl.RiskControlId,
                RiskTitle = riskControl.Risk.RiskTitle,
                ControlTitle = riskControl.Control.ControlTitle,
                ControlMethod = ControlMethodToString(riskControl.ControlMethod)
            };
        }

        public async Task<RiskControl> ToEntity(Guid userId, CreateRiskControlDto dto)
        {
            return new RiskControl
            {
                RiskControlId = Guid.NewGuid(),
                UserId = userId,
                RiskId = dto.RiskId,
                ControlId = dto.ControlId,
                ControlMethod = ParseControlMethod(dto.ControlMethod),
                CreatedBy = userId
            };
        }

        public void UpdateEntity(RiskControl riskControl, Guid userId, UpdateRiskControlDto dto)
        {
            riskControl.ControlMethod = ParseControlMethod(dto.ControlMethod);
            riskControl.LastUpdatedAt = DateTime.UtcNow;
            riskControl.LastUpdatedBy = userId;
        }

        public async Task PatchEntity(RiskControl riskControl, Guid userId, PatchRiskControlDto dto)
        {
            if (!string.IsNullOrEmpty(dto.ControlMethod))
                riskControl.ControlMethod = ParseControlMethod(dto.ControlMethod);
            riskControl.LastUpdatedAt = DateTime.UtcNow;
            riskControl.LastUpdatedBy = userId;
        }

        private ControlMethod ParseControlMethod(string method)
        {
            if (!Enum.TryParse<ControlMethod>(method, true, out var result))
                throw new NotFoundException($"Invalid ControlMethod: {method}");
            return result;
        }

        private string ControlMethodToString(ControlMethod method)
        {
            return method.ToString();
        }
    }
}
