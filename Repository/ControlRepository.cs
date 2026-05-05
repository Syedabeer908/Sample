using Microsoft.EntityFrameworkCore;
using WebApplication1.Entities;
using WebApplication1.Interfaces;

namespace WebApplication1.Repository
{
    public class ControlRepository : IRepository<Control>
    {
        private readonly AppDbContext _context;

        public ControlRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Control>> GetAllAsync() => 
            await _context.Control.AsNoTracking()
                .Include(u => u.User)
                .ToListAsync();

        public async Task<Control?> GetByIdAsync(Guid controlId) =>
            await _context.Control.AsNoTracking()
                .Include(u => u.User)
                .FirstOrDefaultAsync(r => r.ControlId == controlId);

        public async Task AddAsync(Control control)
        {
            await _context.Control.AddAsync(control);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Control control)
        {
            _context.Control.Update(control);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Control control)
        {
            _context.Control.Remove(control);
            await _context.SaveChangesAsync();
        }
    }
}
