using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WebApplication1.Common.Extensions;
using WebApplication1.DTOs;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _service;

        public UserController(UserService service)
        {
            _service = service;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] BaseUpdateUserDto dto)
        {
            var userId = HttpContext.GetUserId();
            var result = await _service.UpdateAsync(id, userId, dto);
            return Ok(result);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchAsync(Guid id, [FromBody] BasePatchUserDto dto)
        {
            var userId = HttpContext.GetUserId();
            var result = await _service.PatchAsync(id, userId, dto);
            return Ok(result);
        }
    }
}
