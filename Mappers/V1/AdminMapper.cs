using Microsoft.AspNetCore.Identity;
using WebApplication1.DTOs;
using WebApplication1.DTOs.Admin.V1;
using WebApplication1.Entities;

namespace WebApplication1.Mappers.V1
{
    public class AdminMapper
    {
        private readonly PasswordHasher<User> _hasher;

        public AdminMapper() 
        {
            _hasher = new PasswordHasher<User>();
        }
        public UserDto ToDto(User user)
        {
            return new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                RoleId = user.RoleId,
                RoleName = user.Role.Name,
                IsActive = user.IsActive
            };
        }

        public User ToEntity(Guid userId, CreateUserDto dto)
        {
            var user = new User
            {
                UserId = Guid.NewGuid(),
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = dto.Password,
                RoleId = dto.RoleId,
                CreatedBy = userId
            };

            user.PasswordHash = _hasher.HashPassword(user, dto.Password);
            return user;
        }

        public void UpdateEntity(User user, Guid userId, BaseUpdateUserDto dto)
        {
            user.Username = dto.Username;
            user.Email = dto.Email;

            if (!string.IsNullOrWhiteSpace(dto.Password))
                user.PasswordHash = _hasher.HashPassword(user, dto.Password);

            user.LastUpdatedAt = DateTime.UtcNow;
            user.LastUpdatedBy = userId;
        }

        public void PatchEntity(User user, Guid userId, BasePatchUserDto dto)
        {
            if (!string.IsNullOrEmpty(dto.Username))
                user.Username = dto.Username;

            if (!string.IsNullOrEmpty(dto.Email))
                user.Email = dto.Email;

            if (!string.IsNullOrEmpty(dto.Password))
                user.PasswordHash = _hasher.HashPassword(user, dto.Password);

            user.LastUpdatedAt = DateTime.UtcNow;
            user.LastUpdatedBy = userId;
        }
    }
}
