using WebApplication1.Entities;
using WebApplication1.DTOs;

namespace WebApplication1.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task<List<RefreshToken>> GetByUserIdAsync(Guid id);

        Task AddAsync(RefreshToken token);
        Task UpdateAsync(RefreshToken token);
        Task UpdateRangeAsync(Guid userId);
        Task DeleteRangeAsync(Guid userId);
    }
}
