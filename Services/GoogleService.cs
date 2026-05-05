using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using WebApplication1.Common.Constants;
using WebApplication1.Common.Exceptions;
using WebApplication1.Common.Parsers;
using WebApplication1.DTOs;
using WebApplication1.Mappers;
using WebApplication1.Settings;

namespace WebApplication1.Services
{
    public class GoogleService
    {
        private readonly ProviderService _providerService;
        private readonly GoogleMapper _mapper;
        private readonly ILogger<GoogleService> _logger;
        private readonly GoogleSettings _googleSettings;
        
        public GoogleService(ProviderService providerService, ILogger<GoogleService> logger, IOptions<GoogleSettings> googleOptions ) 
        {
            _googleSettings = googleOptions.Value;
            _providerService = providerService;
            _mapper = new GoogleMapper();
            _logger = logger;
        }

        public async Task<AuthResponseDto> ValidateAsync(string token, UserInfo userInfo)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(token,
                    new GoogleJsonWebSignature.ValidationSettings
                    {
                        Audience = new[] { _googleSettings.ClientId }
                    }
                );
                var dto = _mapper.ToDto(Providers.Google, payload.Subject,
                    payload.Email, payload.Name, payload.Picture);
                var response = await _providerService.Handle(dto, userInfo);
                return response;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Google token validation failed");
                throw new BadRequestException("Invalid Token");
            }
        }
    }
}
