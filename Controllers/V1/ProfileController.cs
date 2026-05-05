using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebApplication1.Common.Exceptions;
using WebApplication1.Common.Extensions;
using WebApplication1.Common.Results;
using WebApplication1.DTOs;
using WebApplication1.Mappers;
using WebApplication1.Services;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApplication1.Controllers.V1
{
    [Authorize]
    [ApiController]
    [Route("api/profile")]
    public class ProfileController : ControllerBase
    {
        private readonly ProfileService _service;
        private readonly ResultHelper _resultHelper;

        public ProfileController(ProfileService service) 
        { 
            _service = service;
            _resultHelper = new ResultHelper();
        }

        [HttpGet("get-image")]
        public async Task<IActionResult> GetProfileImageAsync()
        {
            var userId = HttpContext.GetUserId();

            var user = await _service.CheckUserExistAndGet(userId);

            var data = _service.GetProfileStream(user);

            if (data == null)
                throw new NotFoundException("No content found");

            return File(data.Value.FileStream, data.Value.FileType);
        }

        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadProfileImageAsync([FromForm] CreateProfileImageDto dto)
        {
            var userId = HttpContext.GetUserId();
            await _service.UpdateProfileImage(userId, dto.File);
            return Ok(_resultHelper.Success());
        }

        [HttpPost("delete-image")]
        public async Task<IActionResult> DeleteProfileImageAsync()
        {
            var userId = HttpContext.GetUserId();
            var user = await _service.CheckUserExistAndGet(userId);

            await _service.DeleteProfile(user);
            return Ok(_resultHelper.Success());
        }

    }
}
