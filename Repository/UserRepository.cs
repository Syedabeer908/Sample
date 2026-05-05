using Microsoft.EntityFrameworkCore;
using WebApplication1.Entities;
using WebApplication1.Interfaces;

namespace WebApplication1.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.User.AsNoTracking().Include(u => u.Role)
                                       .FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.User.AsNoTracking().Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<List<User>> GetByRoleIdAsync(Guid roleId)
        {
            return await _context.User.AsNoTracking().Include(u => u.Role).Where(u => u.RoleId == roleId).ToListAsync();
        }

        public async Task<List<User>> GetAllAsync(int pageSize = 10, string? search = "",
            bool? isActive = null, Guid? lastId = null)
        {
            var query = _context.User.AsNoTracking();

            if (isActive != null) 
            {
                query = query.Where(u => u.IsActive == isActive);
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u =>
                    EF.Functions.Like(u.Username, $"{search}%") ||
                    EF.Functions.Like(u.Email, $"{search}%"));
            }

            if (lastId != null)
            {
                query = query.Where(u => u.UserId > lastId);
            }

            return await query
                .Include(u => u.Role)
                .OrderBy(b => b.UserId)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task AddAsync(User user)
        {
            await _context.User.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.User.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(User user)
        {
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> EmailExistsAsync(string email, Guid? excludeId = null)
        {
            var query = _context.User.AsNoTracking()
                .Where(u => u.Email == email);

            if (excludeId.HasValue)
                query = query.Where(u => u.UserId != excludeId);

            return await query.AnyAsync();
        }
    }
}
