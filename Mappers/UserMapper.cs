using Microsoft.AspNetCore.Identity;
using WebApplication1.DTOs;
using WebApplication1.Entities;

namespace WebApplication1.Mappers
{
    public class UserMapper
    {
        private readonly PasswordHasher<User> _hasher;
        public UserMapper() 
        {
            _hasher = new PasswordHasher<User>();
        }

        public BaseUserDto ToDto(User user)
        {
            return new BaseUserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email
            };
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
