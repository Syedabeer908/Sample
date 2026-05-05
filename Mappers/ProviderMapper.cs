using WebApplication1.Common.Exceptions;
using WebApplication1.DTOs;
using WebApplication1.Entities;

namespace WebApplication1.Mappers
{
    public class ProviderMapper
    {
        public User ToEntity(string username, string email, Role? role)
        {
            if (role == null)
                throw new NotFoundException($"Role '{role?.Name}' not found.");

            var user = new User
            {
                UserId = Guid.NewGuid(),
                Username = username,
                Email = email,
                RoleId = role.RoleId,
                TokenVersion = 0
            };
            return user;
        }

        public ExternalLogin ToExternalEntity(ExternalLoginDto dto, Guid userId)
        {
            var entity = new ExternalLogin
            {
                ExternalLoginId = Guid.NewGuid(),
                UserId = userId,
                Name = dto.Name,
                Email = dto.Email,
                Provider = dto.Provider,
                ProviderKey = dto.ProviderKey,
                PictureUrl = dto.PictureUrl,
            };
            return entity;
        }
    }
}
