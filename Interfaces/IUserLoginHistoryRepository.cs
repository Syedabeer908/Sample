using WebApplication1.Entities;

namespace WebApplication1.Interfaces
{
    public interface IUserLoginHistoryRepository
    {
        Task<List<UserLoginHistory>> GetByUserIdAsync(Guid userId);
        Task<List<UserLoginHistory>> GetByIpAddressAsync(string ipAddress);
        Task<List<UserLoginHistory>> GetByDeviceInfoAsync(string ipAddress);
        Task AddAsync(UserLoginHistory loginHistory);
        Task DeleteRangeAsync(Guid userId);
    }
}
