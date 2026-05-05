using Microsoft.EntityFrameworkCore;
using WebApplication1.Entities;
using WebApplication1.Interfaces;

namespace WebApplication1.Repository
{
    public class RiskControlRepository : IRepository<RiskControl>
    {
        private readonly AppDbContext _context;

        public RiskControlRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<RiskControl>> GetAllAsync() => 
          await _context.RiskControl.AsNoTracking()
                .Include(u => u.User)
                .Include(rc => rc.Risk)
                .Include(rc => rc.Control)
                .ToListAsync();

        public async Task<RiskControl?> GetByIdAsync(Guid riskControlId) =>
            await _context.RiskControl
                .Include(u => u.User).AsNoTracking()
                .Include(rc => rc.Risk)
                .Include(rc => rc.Control)
                .FirstOrDefaultAsync(rc => rc.RiskControlId == riskControlId);

        public async Task AddAsync(RiskControl riskControl)
        {
            await _context.RiskControl.AddAsync(riskControl);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(RiskControl riskControl)
        {
            _context.RiskControl.Update(riskControl);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(RiskControl riskControl)
        {
            _context.RiskControl.Remove(riskControl);
            await _context.SaveChangesAsync();
        }
    }
}
