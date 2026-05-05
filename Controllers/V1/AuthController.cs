using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Common.Parsers;
using WebApplication1.Common.Results;
using WebApplication1.DTOs;
using WebApplication1.Services;

namespace WebApplication1.Controllers.V1
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ResultHelper _resultHelper;
        private readonly Parser _parser;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _resultHelper = new ResultHelper();
            _parser = new Parser();
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AuthRegisterDto dto)
        {
            var createdUser = await _authService.RegisterAsync(dto);
            return Ok(_resultHelper.Success<AuthResponseDto>(createdUser));
        }

        [HttpPost("login")]
        public async Task<IActionResult> login(AuthLoginDto dto)
        {
            var userInfo = _parser.GetIpAndDeviceInfo(HttpContext);
            var authResponse = await _authService.LoginAsync(dto, userInfo);
            return Ok(_resultHelper.Success<AuthResponseDto>(authResponse));
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenDto dto)
        {
            var authResponse = await _authService.Refresh(dto);
            return Ok(_resultHelper.Success<AuthResponseDto>(authResponse));
        }
	
	    [HttpGet("whoami")]
	    public IActionResult WhoAmI()
	    {
    	    return Ok(new
    		{
                pod = Environment.MachineName,
        		time = DateTime.UtcNow
    	    });
        }
    }
}
