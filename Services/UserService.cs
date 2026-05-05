using WebApplication1.Common.Results;
using WebApplication1.DTOs;
using WebApplication1.Interfaces;
using WebApplication1.Mappers;

namespace WebApplication1.Services
{
    public class UserService
    {
        private readonly IUserRepository _repo;
        private readonly UserDomainService _userDomainService;
        private readonly UserMapper _mapper;

        public UserService(IUserRepository repo, UserDomainService userDomainService)
        {
            _repo = repo;
            _userDomainService = userDomainService;
            _mapper = new UserMapper();
        }

        public async Task<ResultT<BaseUserDto>> UpdateAsync(Guid id, Guid userId, BaseUpdateUserDto dto)
        {
            var user = await _userDomainService.CheckUserExistAndGet(id);

            await _userDomainService.CheckEmailUnique(dto.Email, user.UserId);

            _mapper.UpdateEntity(user, userId, dto);

            await _repo.UpdateAsync(user);

            return ResultT<BaseUserDto>.Success(_mapper.ToDto(user));
        }

        public async Task<ResultT<BaseUserDto>> PatchAsync(Guid id, Guid userId, BasePatchUserDto dto)
        {
            var user = await _userDomainService.CheckUserExistAndGet(id);

            if (!string.IsNullOrEmpty(dto.Email))
                await _userDomainService.CheckEmailUnique(dto.Email, user.UserId);

            _mapper.PatchEntity(user, userId, dto);

            await _repo.UpdateAsync(user);

            return ResultT<BaseUserDto>.Success(_mapper.ToDto(user));
        }
    }
}
