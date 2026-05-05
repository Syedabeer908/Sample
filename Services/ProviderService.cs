using WebApplication1.Common.Parsers;
using WebApplication1.DTOs;
using WebApplication1.Entities;
using WebApplication1.Interfaces;
using WebApplication1.Mappers;

namespace WebApplication1.Services
{
    
    public class ProviderService
    {
        private readonly IProviderRepository _providerRepo;
        private readonly AuthService _authService;
        private readonly ProviderMapper _mapper; 

        public  ProviderService(IProviderRepository providerRepo, AuthService authService)
        {
            _providerRepo = providerRepo;
            _authService = authService;
            _mapper = new ProviderMapper();
        }

        public async Task<AuthResponseDto> Handle(ExternalLoginDto dto, UserInfo userInfo)
        {
            var checkExternal = await _providerRepo.GetByProviderAndProviderKeyAsync(dto.Provider,
                dto.ProviderKey);

            if (checkExternal == null) 
                return await HandleRegisterAsync(dto);

            return await HandleLoginAsync(checkExternal, userInfo);
        }

        public async Task<AuthResponseDto> HandleRegisterAsync(ExternalLoginDto dto)
        {
            var user = await _authService.CheckUserExistAndGetByEmail(dto.Email);
            User newUser;
            string roleName;
            Guid userId;

            if (user == null)
            {
                var role = await _authService.GetUserRole();
                newUser = _mapper.ToEntity(dto.Name, dto.Email, role);
                await _authService.AddUser(newUser);
                roleName = role.Name;
                userId = newUser.UserId;
            }
            else
            {
                roleName = user.Role.Name;
                userId = user.UserId;
                newUser = user;
            }

            var externalEntity = _mapper.ToExternalEntity(dto, userId);
            await _providerRepo.AddAsync(externalEntity);
                
            _authService.BackgroundRegisterWork(userId);

            return await _authService.IssueTokens(newUser, roleName);
        }

        public async Task<AuthResponseDto> HandleLoginAsync(ExternalLogin externalLogin, UserInfo userInfo)
        {
            var user = await _authService.CheckUserExistAndGetById(externalLogin.UserId);

            _authService.BackgroundLoginWork(user.UserId, userInfo);

            return await _authService.IssueTokens(user, user.Role.Name);
        }
    }
}
