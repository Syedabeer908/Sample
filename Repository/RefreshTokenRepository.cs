using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using WebApplication1.DTOs;
using WebApplication1.Entities;
using WebApplication1.Interfaces;

namespace WebApplication1.Repository
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _context;

        public RefreshTokenRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _context.RefreshToken.AsNoTracking().
                FirstOrDefaultAsync(r => r.Token == token);
        }


        public async Task<List<RefreshToken>> GetByUserIdAsync(Guid userId)
        {
            var query = _context.RefreshToken.AsNoTracking();
            query = query.Where(f => f.UserId == userId);
            return await query.ToListAsync();
        }

        public async Task AddAsync(RefreshToken refreshToken)
        {
            await _context.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(RefreshToken refreshToken)
        {
            _context.Update(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(Guid userId)
        {
            await _context.RefreshToken
                .Where(x => x.UserId == userId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.IsRevoked, true)
                );
        }

        public async Task DeleteRangeAsync(Guid userId)
        {
            await _context.RefreshToken.Where(x => x.UserId == userId).ExecuteDeleteAsync();
        }
    }
}
