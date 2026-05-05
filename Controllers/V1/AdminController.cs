using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Common.Exceptions;
using WebApplication1.Common.Extensions;
using WebApplication1.Common.Results;
using WebApplication1.DTOs;
using WebApplication1.DTOs.Admin.V1;
using WebApplication1.Mappers.V1;
using WebApplication1.Services;


namespace WebApplication1.Controllers.V1
{
    [Authorize(Policy = "AdminOnly")]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}")]
    public class AdminController : ControllerBase
    {
        private readonly AdminService _service;
        private readonly AdminMapper _mapper;
        private readonly ResultHelper _resultHelper;

        public AdminController(AdminService service)
        {
            _service = service;
            _mapper = new AdminMapper();
            _resultHelper = new ResultHelper();
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllAsync( [FromQuery] string? search,
                [FromQuery] bool? isActive,
                [FromQuery] Guid? lastId,
                [FromQuery] int pageSize = 10 )
        {
            var data = await _service.GetAllAsync(pageSize, search, isActive, lastId);

            if (data == null) 
                throw new NotFoundException("No content found");

            var result = data.Select(item => _mapper.ToDto(item)).ToList();
            return Ok(_resultHelper.Success<UserDto>(result));
        }

        [HttpGet("users/{id}", Name = "GetUserById")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }
        
        [HttpPost("users")]
        public async Task<IActionResult> AddAsync([FromBody] CreateUserDto dto)
        {
            var userId = HttpContext.GetUserId();
            var entity = _mapper.ToEntity(userId, dto);

            var data = await _service.AddAsync(userId, entity);

            if (data == null)
                throw new NotFoundException("No content found");

            var dtoResult = _mapper.ToDto(data);
            var result = _resultHelper.Success<UserDto>(dtoResult);

            return CreatedAtRoute(
                "GetUserById",
                new { id = dtoResult.UserId },
                result
            );
        }

        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] BaseUpdateUserDto dto)
        {
            var userId = HttpContext.GetUserId();
            var result = await _service.UpdateAsync(id, userId, dto);
            return Ok(result);
        }

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var userId = HttpContext.GetUserId();
            var result = await _service.DeleteAsync(id, userId);
            return Ok(result);
        }

        [HttpPatch("users/{id}/restore")]
        public async Task<IActionResult> RestoreAsync(Guid id)
        {
            var userId = HttpContext.GetUserId();
            var result = await _service.RestoreAsync(id, userId);
            return Ok(result);
        }

        [HttpPatch("users/{id}")]
        public async Task<IActionResult> PatchAsync(Guid id, [FromBody] BasePatchUserDto dto)
        {
            var userId = HttpContext.GetUserId();
            var result = await _service.PatchAsync(id, userId, dto);
            return Ok(result);
        }

        [HttpPatch("users/{id}/disable")]
        public async Task<IActionResult> DisableUserAsync(Guid id)
        {
            var userId = HttpContext.GetUserId();
            var result = await _service.DisableUserAsync(id, userId);
            return Ok(result);
        }

        [HttpPatch("users/{id}/enable")]
        public async Task<IActionResult> EnableUserAsync(Guid id)
        {
            var userId = HttpContext.GetUserId();
            var result = await _service.EnableUserAsync(id, userId);
            return Ok(result);
        }

        [HttpPatch("users/{id}/logout")]
        public async Task<IActionResult> ForceUserLogoutAsync(Guid id)
        {
            var userId = HttpContext.GetUserId();
            var result = await _service.ForceUserLogoutAsync(id, userId);
            return Ok(result);
        }
    }
}
