using Microsoft.EntityFrameworkCore;
using WebApplication1.Entities;
using WebApplication1.Interfaces;

namespace WebApplication1.Repository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext _context;

        public RoleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Role?> GetByIdAsync(Guid id)
        {
            return await _context.Role.AsNoTracking().FirstOrDefaultAsync(r => r.RoleId == id);
        }

        public async Task<Role?> GetByNameAsync(string name)
        {
            return await _context.Role.AsNoTracking().FirstOrDefaultAsync(r => r.Name == name);
        }

        public async Task<List<Role>> GetAllAsync()
        {
            return await _context.Role.AsNoTracking().ToListAsync();
        }

        public async Task AddAsync(Role role)
        {
            await _context.Role.AddAsync(role);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Role role)
        {
            _context.Role.Update(role);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Role role)
        {
            _context.Role.Remove(role);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> RoleNameExistsAsync(string roleName, Guid? excludeId = null)
        {
            var query = _context.Role.AsNoTracking()
                .Where(u => u.Name == roleName);

            if (excludeId.HasValue)
                query = query.Where(u => u.RoleId != excludeId);

            return await query.AnyAsync();
                
        }
    }
}
