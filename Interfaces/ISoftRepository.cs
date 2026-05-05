using static WebApplication1.Data.Soft;

namespace WebApplication1.Interfaces
{
    public interface ISoftRepository
    {
       
        Task SoftRoleAsync(Guid roleId, SoftValues values);
        Task SoftUserAsync(Guid userId, SoftValues values);
        Task SoftRiskAsync(Guid riskId, SoftValues values);
        Task SoftControlAsync(Guid controlId, SoftValues values);
        Task SoftRiskControlAsync(Guid riskcontrolId, SoftValues values);
    }
}
