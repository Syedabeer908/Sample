using WebApplication1.Entities;

namespace WebApplication1.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role?> GetByIdAsync(Guid id);
        Task<Role?> GetByNameAsync(string name);
        Task<List<Role>> GetAllAsync();

        Task AddAsync(Role role);
        Task UpdateAsync(Role role);
        Task DeleteAsync(Role role);
        Task<bool> RoleNameExistsAsync(string name, Guid? excludeId = null);
    }
}
