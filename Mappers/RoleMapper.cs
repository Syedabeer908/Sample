using WebApplication1.DTOs.Role;
using WebApplication1.Entities;

namespace WebApplication1.Mappers
{
    public class RoleMapper
    {
        public RoleDto ToDto(Role role)
        {
            return new RoleDto
            {
                RoleId = role.RoleId,
                RoleName = role.Name
            };
        }

        public Role ToEntity(Guid userId, CreateRoleDto dto)
        {
            return new Role
            {
                RoleId = Guid.NewGuid(),
                Name = dto.RoleName,
                CreatedBy = userId
            };
        }

        public void UpdateEntity(Role role, Guid userId, UpdateRoleDto dto)
        {
            role.Name = dto.RoleName;
            role.LastUpdatedAt = DateTime.UtcNow;
            role.LastUpdatedBy = userId;
        }

        public void PatchEntity(Role role, Guid userId, PatchRoleDto dto)
        {
            if (!string.IsNullOrEmpty(dto.RoleName))
                role.Name = dto.RoleName;
            role.LastUpdatedAt = DateTime.UtcNow;
            role.LastUpdatedBy = userId;
        }
    }
}
