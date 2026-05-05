using Microsoft.AspNetCore.Identity;
using WebApplication1.Common.Exceptions;
using WebApplication1.Entities;
using WebApplication1.DTOs.Admin.V2;

namespace WebApplication1.Mappers.V2
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
                IsActive = user.IsActive
            };
        }

        public User ToEntity(Guid userId, CreateUserDto dto, Role? role)
        {
            if (role == null)
                throw new NotFoundException($"Role '{role?.Name}' not found.");

            var user = new User
            {
                UserId = Guid.NewGuid(),
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = dto.Password,
                RoleId = role.RoleId,
                CreatedBy = userId
            };

            user.PasswordHash = _hasher.HashPassword(user, dto.Password);
            return user;
        }
    }
}
