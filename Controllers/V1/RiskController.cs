using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Common.Extensions;
using WebApplication1.DTOs.Risk;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RiskController : ControllerBase
    {
        private readonly RiskService _service;

        public RiskController(RiskService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}", Name = "GetRiskById")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] CreateRiskDto dto)
        {
            var userId = HttpContext.GetUserId();
            var result = await _service.AddAsync(userId, dto);
            if(result.Result == null)
                return BadRequest(result);
            return CreatedAtRoute("GetRiskById", new { id = result.Result.RiskId }, result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateRiskDto dto)
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
        public async Task<IActionResult> PatchAsync(Guid id, [FromBody] PatchRiskDto dto)
        {
            var userId = HttpContext.GetUserId();
            var result = await _service.PatchAsync(id, userId, dto);
            return Ok(result);
        }
    }
}
