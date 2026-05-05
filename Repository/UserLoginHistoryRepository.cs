using Microsoft.EntityFrameworkCore;
using WebApplication1.Entities;
using WebApplication1.Interfaces;

namespace WebApplication1.Repository
{
    public class UserLoginHistoryRepository : IUserLoginHistoryRepository
    {
        private readonly AppDbContext _context;

        public UserLoginHistoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserLoginHistory>> GetByUserIdAsync(Guid userId)
        {
            return await _context.UserLoginHistory
                .Where(x => x.UserId == userId).AsNoTracking()
                .OrderByDescending(x => x.LoginTime)
                .ToListAsync();
        }

        public async Task<List<UserLoginHistory>> GetByIpAddressAsync(string ipAddress)
        {
            return await _context.UserLoginHistory.AsNoTracking()
                .Where(x => x.IpAddress == ipAddress)
                .OrderByDescending(x => x.LoginTime)
                .ToListAsync();
        }

        public async Task<List<UserLoginHistory>> GetByDeviceInfoAsync(string deviceInfo)
        {
            return await _context.UserLoginHistory.AsNoTracking()
                .Where(x => x.DeviceInfo == deviceInfo)
                .OrderByDescending(x => x.LoginTime)
                .ToListAsync();
        }

        public async Task AddAsync(UserLoginHistory loginHistory)
        {
            await _context.UserLoginHistory.AddAsync(loginHistory);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(Guid userId)
        {
            await _context.UserLoginHistory.Where(x => x.UserId == userId).ExecuteDeleteAsync();
        }

    }
}
