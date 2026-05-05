using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Common.Extensions;
using WebApplication1.DTOs.Role;
using WebApplication1.Services;

namespace WebApplication1.Controllers.V1
{
    [Authorize(Policy = "AdminOnly")]
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly RoleService _service;

        public RoleController(RoleService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}", Name = "GetRoleById")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] CreateRoleDto dto)
        {
            var userId = HttpContext.GetUserId();
            var result = await _service.AddAsync(userId, dto);
            if (result.Result == null)
                return BadRequest(result);
            return CreatedAtRoute("GetRoleById", new { id = result.Result.RoleId }, result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateRoleDto dto)
        {
            var userId = HttpContext.GetUserId();
            var result = await _service.UpdateAsync(id, userId, dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var userId = HttpContext.GetUserId();
            var result = await _service.DeleteAsync(id, userId);
            return Ok(result);
        }

        [HttpPatch("{id}/restore")]
        public async Task<IActionResult> RestoreAsync(Guid id)
        {
            var userId = HttpContext.GetUserId();
            var result = await _service.RestoreAsync(id, userId);
            return Ok(result);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchAsync(Guid id, [FromBody] PatchRoleDto dto)
        {
            var userId = HttpContext.GetUserId();
            var result = await _service.PatchAsync(id, userId, dto);
            return Ok(result);
        }
    }
}
