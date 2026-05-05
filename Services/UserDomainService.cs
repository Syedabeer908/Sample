using WebApplication1.Common.Exceptions;
using WebApplication1.Entities;
using WebApplication1.Interfaces;

namespace WebApplication1.Services
{
    public class UserDomainService
    {
        private readonly IUserRepository _repo;
        private readonly IRoleRepository _roleRepo;

        public UserDomainService(IUserRepository repo, IRoleRepository roleRepo)
        {
            _repo = repo;
            _roleRepo = roleRepo;
        }

        //private async 
        public async Task CheckEmailUnique(string email, Guid? excludeId = null)
        {
            var exists = await _repo.EmailExistsAsync(email, excludeId);

            if (exists)
                throw new BadRequestException($"Invalid Email '{email}'.");
        }

        public async Task<User> CheckUserExistAndGet(Guid targetUserId)
        {
            var user = await _repo.GetByIdAsync(targetUserId);

            if (user == null)
                throw new NotFoundException($"User with id {targetUserId} not found.");
            return user;
        }

        public async Task<Role?> GetRoleByIdAsync(Guid roleId)
        {
            return await _roleRepo.GetByIdAsync(roleId);
        }

        public async Task<Role?> GetRoleByNameAsync(string roleName)
        {
            return await _roleRepo.GetByNameAsync(roleName);
        }
    }
}
