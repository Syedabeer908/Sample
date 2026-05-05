using Microsoft.Extensions.Options;
using WebApplication1.Common.Exceptions;
using WebApplication1.Common.Results;
using WebApplication1.Data;
using WebApplication1.DTOs.Role;
using WebApplication1.Entities;
using WebApplication1.Interfaces;
using WebApplication1.Mappers;
using WebApplication1.Settings;

namespace WebApplication1.Services
{
    public class RoleService 
    {
        private readonly IRoleRepository _repo;
        private readonly IUserRepository _userRepo;
        private readonly ISoftRepository _softRepo;
        private readonly RoleSettings _roleSettings;
        private readonly RoleMapper _mapper;
        public RoleService(IRoleRepository repo, IUserRepository userRepo,
            ISoftRepository softRepo, IOptions<RoleSettings> roleOptions)
        {
            _repo = repo;
            _userRepo = userRepo;
            _softRepo = softRepo;
            _roleSettings = roleOptions.Value;
            _mapper = new RoleMapper();
        }

        private async Task<Role> CheckRoleExistAndGet(Guid id)
        {
            var role = await _repo.GetByIdAsync(id);

            if (role == null)
                throw new NotFoundException($"Role with id {id} not found.");

            return role;
        }

        private async Task CheckRoleNameUnique(string roleName, Guid? excludeId = null)
        {
            var exists = await _repo.RoleNameExistsAsync(roleName, excludeId);
            if (exists)
                throw new BadRequestException($"Role Name '{roleName}' is already in use.");
        }

        private async Task CheckRoleInUse(Guid roleId)
        {
            var userWithRole = await _userRepo.GetByRoleIdAsync(roleId);
            if(userWithRole.Any())
            {
                throw new BadRequestException($"Cannot delete role with id {roleId} because it is assigned to one or more users.");
            }
        }

        private async Task CheckRoleNotStartupRole(Role role)
        {
            if (role.Name.Equals(_roleSettings.Admin, StringComparison.OrdinalIgnoreCase) ||
                role.Name.Equals(_roleSettings.User, StringComparison.OrdinalIgnoreCase))
            {
                throw new BadRequestException($"Cannot delete startup role '{role.Name}'.");
            }
            await CheckRoleInUse(role.RoleId);
        }

        private async Task RunSoftService(Guid id, bool action, Guid actionBy)
        {
            var soft = new Soft();
            var values = soft.SoftValuesSetter(action, DateTime.UtcNow, actionBy);
            await _softRepo.SoftRoleAsync(id, values);
        }

        public async Task<ResultT<List<RoleDto>>> GetAllAsync()
        {
            var roles = await _repo.GetAllAsync();
            var roleDtos = roles.Select(r => _mapper.ToDto(r)).ToList();
            return ResultT<List<RoleDto>>.Success(roleDtos);
        }

        public async Task<ResultT<RoleDto>> GetByIdAsync(Guid id)
        {
            var role = await CheckRoleExistAndGet(id);
            return ResultT<RoleDto>.Success(_mapper.ToDto(role));
        }

        public async Task<ResultT<RoleDto>> AddAsync(Guid userId, CreateRoleDto dto)
        {
            await CheckRoleNameUnique(dto.RoleName);
            var role = _mapper.ToEntity(userId, dto);

            await _repo.AddAsync(role);

            return ResultT<RoleDto>.Success(_mapper.ToDto(role));
        }

        public async Task<ResultT<RoleDto>> UpdateAsync(Guid id, Guid userId, UpdateRoleDto dto)
        {
            var role = await CheckRoleExistAndGet(id);

            await CheckRoleNameUnique(dto.RoleName, role.RoleId);

            _mapper.UpdateEntity(role, userId, dto);

            await _repo.UpdateAsync(role);

            return ResultT<RoleDto>.Success(_mapper.ToDto(role));
        }

        public async Task<Result> DeleteAsync(Guid id, Guid userId)
        {
            var role = await CheckRoleExistAndGet(id);
            await CheckRoleNotStartupRole(role);

            await RunSoftService(id, true, userId);
            return Result.Success();
        }

        public async Task<Result> RestoreAsync(Guid id, Guid userId)
        {
            var role = await CheckRoleExistAndGet(id);
            await CheckRoleNotStartupRole(role);

            await RunSoftService(id, false, userId);
            return Result.Success();
        }

        public async Task<ResultT<RoleDto>> PatchAsync(Guid id, Guid userId, PatchRoleDto dto)
        {
            var role = await CheckRoleExistAndGet(id);

            if (!string.IsNullOrEmpty(dto.RoleName))
                await CheckRoleNameUnique(dto.RoleName, role.RoleId);

            _mapper.PatchEntity(role, userId, dto);

            await _repo.UpdateAsync(role);
            return ResultT<RoleDto>.Success(_mapper.ToDto(role));
        }
    }
}
