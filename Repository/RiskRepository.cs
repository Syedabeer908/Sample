using Microsoft.EntityFrameworkCore;
using WebApplication1.Entities;
using WebApplication1.Interfaces;

namespace WebApplication1.Repository
{
    public class RiskRepository : IRepository<Risk>
    {
        private readonly AppDbContext _context;

        public RiskRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Risk>> GetAllAsync() => 
            await _context.Risk.AsNoTracking()
                .Include(u => u.User)
                .ToListAsync();

        public async Task<Risk?> GetByIdAsync(Guid riskId) =>
            await _context.Risk.AsNoTracking()
                .Include(u => u.User)
                .FirstOrDefaultAsync(r => r.RiskId == riskId);

        public async Task AddAsync(Risk risk)
        {
            await _context.Risk.AddAsync(risk);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Risk risk)
        {
            _context.Risk.Update(risk);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Risk risk)
        {
            _context.Risk.Remove(risk);
            await _context.SaveChangesAsync();
        }
    }
}
