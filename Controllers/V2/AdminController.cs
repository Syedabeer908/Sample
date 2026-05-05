using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Common.Exceptions;
using WebApplication1.Common.Extensions;
using WebApplication1.Common.Results;
using WebApplication1.DTOs.Admin.V2;
using WebApplication1.Mappers.V2;
using WebApplication1.Services;


namespace WebApplication1.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [ApiController]
    [ApiVersion("2.0")]
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
        public async Task<IActionResult> GetAllAsync([FromQuery] string? search,
                [FromQuery] bool? isActive,
                [FromQuery] Guid? lastId,
                [FromQuery] int pageSize = 10)
        {
            var data = await _service.GetAllAsync(pageSize, search, isActive, lastId);

            if (data == null)
                throw new NotFoundException("No content found");

            var result = data.Select(item => _mapper.ToDto(item)).ToList();
            return Ok(_resultHelper.Success<UserDto>(result));
        }

        [HttpPost("users")]
        public async Task<IActionResult> AddAsync([FromBody] CreateUserDto dto)
        {
            var userId = HttpContext.GetUserId();
            var role =  await _service.GetRole();
            var entity = _mapper.ToEntity(userId, dto, role);

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
    }
}
