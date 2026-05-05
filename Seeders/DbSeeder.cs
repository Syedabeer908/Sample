using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebApplication1.Entities;
using WebApplication1.Settings;

namespace WebApplication1.Seeders
{
    public class DbSeeder
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<User> _hasher;
        private readonly RoleSettings _roleSettings;
        public DbSeeder(AppDbContext context, IOptions<RoleSettings> roleOptions)
        {
            _context = context;
            _hasher = new PasswordHasher<User>();
            _roleSettings= roleOptions.Value;
        }
        public async Task SeedAsync()
        {
            if (!await _context.Role.AnyAsync())
            {
                _context.Role.AddRange(
                    new Role { RoleId = Guid.NewGuid(), Name = _roleSettings.Admin },
                    new Role { RoleId = Guid.NewGuid(), Name = _roleSettings.User }
                );

                await _context.SaveChangesAsync();
            }

            var adminRole = await _context.Role
                .FirstOrDefaultAsync(r => r.Name == _roleSettings.Admin);

            if (adminRole == null)
                return;

            bool adminExists = await _context.User
                .AnyAsync(u => u.RoleId == adminRole.RoleId);

            if (adminExists)
                return;

            var adminUser = new User
            {
                UserId = Guid.NewGuid(),
                Username = "admin",
                Email = "admin@transviti.com",
                RoleId = adminRole.RoleId,
                IsActive = true
            };

            adminUser.PasswordHash = _hasher.HashPassword(adminUser, "Abeer@l1");

            _context.User.Add(adminUser);
            await _context.SaveChangesAsync();
           
        }
    }
}
