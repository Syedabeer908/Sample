using Microsoft.EntityFrameworkCore;
using WebApplication1.Entities;
using WebApplication1.Interfaces;

namespace WebApplication1.Repository
{
    public class ProviderRepository : IProviderRepository
    {
        private readonly AppDbContext _context;

        public ProviderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ExternalLogin?> GetByProviderAndProviderKeyAsync(string provider, string providerKey)
        {
            return await _context.ExternalLogin.AsNoTracking()
                .Where(r => r.Provider == provider)
                .Where(r => r.ProviderKey == providerKey)
                .FirstOrDefaultAsync();
        }

        public async Task<List<ExternalLogin>?> GetByUserIdAsync(Guid userId)
        {
            return await _context.ExternalLogin.AsNoTracking()
                .Where(r => r.UserId == userId).ToListAsync();
        }

        public async Task AddAsync(ExternalLogin externalLogin)
        {
            await _context.ExternalLogin.AddAsync(externalLogin);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ExternalLogin externalLogin)
        {
            _context.ExternalLogin.Update(externalLogin);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ExternalLogin externalLogin)
        {
            _context.ExternalLogin.Remove(externalLogin);
            await _context.SaveChangesAsync();
        }
    }
}
