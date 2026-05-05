
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Common.Results;
using WebApplication1.Common.Parsers;
using WebApplication1.DTOs;
using WebApplication1.Services;
using WebApplication1.Mappers;

namespace WebApplication1.Controllers.V1
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/auth")]
    public class GoogleController : ControllerBase
    {
        private readonly GoogleService _service;
        private readonly ResultHelper _resultHelper;
        private readonly Parser _parser;

        public GoogleController(GoogleService service) 
        {
            _service = service;
            _resultHelper = new ResultHelper();
            _parser = new Parser();
        }

        [HttpPost("google")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            var userInfo = _parser.GetIpAndDeviceInfo(HttpContext);
            var result = await _service.ValidateAsync(request.IdToken, userInfo);
            return Ok(_resultHelper.Success<AuthResponseDto>(result));
        }
    }
}
