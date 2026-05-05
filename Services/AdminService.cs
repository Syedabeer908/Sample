using Microsoft.Extensions.Options;
using System.Text.Json;
using WebApplication1.Common.Exceptions;
using WebApplication1.Common.Results;
using WebApplication1.Data;
using WebApplication1.Entities;
using WebApplication1.DTOs;
using WebApplication1.DTOs.Admin.V1;
using WebApplication1.Interfaces;
using WebApplication1.Mappers.V1;
using WebApplication1.Settings;

namespace WebApplication1.Services
{
    public class AdminService
    {
        private readonly IUserRepository _repo;
        private readonly ISoftRepository _softRepo;
        private readonly UserDomainService _userDomainService;
        private readonly RedisService _redis;
        private readonly RoleSettings _roleSettings;
        private readonly AdminMapper _mapper;

        public AdminService(IUserRepository repo, ISoftRepository softRepo,
            UserDomainService userDomainService, RedisService redis, IOptions<RoleSettings> roleOptions)
        {
            _repo = repo;
            _softRepo = softRepo;
            _userDomainService = userDomainService;
            _redis = redis;
            _roleSettings = roleOptions.Value;
            _mapper = new AdminMapper();
        }

        private async Task CheckIsAdmin(string roleName)
        {
            if (roleName != null && roleName == _roleSettings.Admin)
                throw new BadRequestException("You are not allowed to update/delete this user.");
        }

        private async Task<string> CreateValueForCacheAfterUserUpdate(User user)
        {
            var json = JsonSerializer.Serialize(new RedisUserDto
            {
                IsActive = user.IsActive,
                IsDeleted = user.IsDeleted,
                TokenVersion = user.TokenVersion
            });
            return json;
        }

        private async Task CreateOrUpdateCacheAfterUserUpdate(Guid userId, User user)
        {
            var prefix = _roleSettings.User;
            var data = await CreateValueForCacheAfterUserUpdate(user);
            await _redis.SetAsync(prefix, userId.ToString(), data, TimeSpan.FromHours(1));
            
        }

        private async Task RunSoftService(Guid id, bool action, Guid actionBy)
        {
            var soft = new Soft();
            var values = soft.SoftValuesSetter(action, DateTime.UtcNow, actionBy);
            await _softRepo.SoftUserAsync(id, values);
        }

        public async Task<Role?> GetRole(string? roleName = null)
        {
            if (roleName == null)
                roleName = _roleSettings.User;

            var role = await _userDomainService.GetRoleByNameAsync(roleName);
            return role;
        }

        public async Task<List<User>> GetAllAsync(int pageSize, string? search,
            bool? isActive, Guid? lastId)
        {
            var users = await _repo.GetAllAsync(pageSize, search, isActive, lastId);
            var usersDtos = users.Select(u => u).ToList();
            return usersDtos;
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _userDomainService.CheckUserExistAndGet(id);
        }

        public async Task<User> AddAsync(Guid userId, User user)
        {
            await _userDomainService.CheckEmailUnique(user.Email);

            var role = await _userDomainService.GetRoleByIdAsync(user.RoleId);

            if (role == null)
                throw new NotFoundException($"RoleId '{user.RoleId}' not found.");

            await _repo.AddAsync(user);

            var data = await _userDomainService.CheckUserExistAndGet(user.UserId);

            return data;
        }

        public async Task<ResultT<UserDto>> UpdateAsync(Guid id, Guid userId, BaseUpdateUserDto dto)
        {
            var user = await _userDomainService.CheckUserExistAndGet(id);

            await _userDomainService.CheckEmailUnique(dto.Email, user.UserId);

            _mapper.UpdateEntity(user, userId, dto);
            
            await _repo.UpdateAsync(user);

            return ResultT<UserDto>.Success(_mapper.ToDto(user));
        }

        public async Task<ResultT<UserDto>> PatchAsync(Guid id, Guid userId, BasePatchUserDto dto)
        {
            var user = await _userDomainService.CheckUserExistAndGet(id);

            if (!string.IsNullOrEmpty(dto.Email))
                await _userDomainService.CheckEmailUnique(dto.Email, user.UserId);

            _mapper.PatchEntity(user, userId, dto);

            await _repo.UpdateAsync(user);

            return ResultT<UserDto>.Success(_mapper.ToDto(user));
        }

        public async Task<Result> DeleteAsync(Guid id, Guid userId)
        {
            var user = await _userDomainService.CheckUserExistAndGet(id);

            await CheckIsAdmin(user.Role.Name);
            user.IsDeleted = true;

            await RunSoftService(id, user.IsDeleted, userId);
            await CreateOrUpdateCacheAfterUserUpdate(id, user);
            return Result.Success();
        }

        public async Task<Result> RestoreAsync(Guid id, Guid userId)
        {
            var user = await _userDomainService.CheckUserExistAndGet(id);

            await CheckIsAdmin(user.Role.Name);
            user.IsDeleted = false;

            await RunSoftService(id, user.IsDeleted, userId);
            await CreateOrUpdateCacheAfterUserUpdate(id, user);
            return Result.Success();
        }

        public async Task<Result> DisableUserAsync(Guid id, Guid userId)
        {
            var user = await _userDomainService.CheckUserExistAndGet(id);
            await CheckIsAdmin(user.Role.Name);

            user.IsActive = false;
            user.LastUpdatedAt = DateTime.UtcNow;
            user.LastUpdatedBy = userId;  
            
            await _repo.UpdateAsync(user);
            await CreateOrUpdateCacheAfterUserUpdate(id, user);
            return Result.Success();
        }

        public async Task<Result> EnableUserAsync(Guid id, Guid userId)
        {
            var user = await _userDomainService.CheckUserExistAndGet(id);
            await CheckIsAdmin(user.Role.Name);

            user.IsActive = true;
            user.LastUpdatedAt = DateTime.UtcNow;
            user.LastUpdatedBy = userId;

            await _repo.UpdateAsync(user);
            await CreateOrUpdateCacheAfterUserUpdate(id, user);
            return Result.Success();
        }

        public async Task<Result> ForceUserLogoutAsync(Guid id, Guid userId)
        {
            var user = await _userDomainService.CheckUserExistAndGet(id);
            await CheckIsAdmin(user.Role.Name);

            user.TokenVersion += 1;
            user.LastUpdatedAt = DateTime.UtcNow;
            user.LastUpdatedBy = userId;

            await _repo.UpdateAsync(user);
            await CreateOrUpdateCacheAfterUserUpdate(id, user);
            return Result.Success();
        }
    }
}
