using WebApplication1.DTOs;

namespace WebApplication1.Mappers
{
    public class GoogleMapper
    {
        public ExternalLoginDto ToDto(string providerName, string ProviderKey, string email,
            string name, string? picture = null)
        {
            return new ExternalLoginDto
            {
                Provider = providerName,
                ProviderKey = ProviderKey,
                Email = email,
                Name = name,
                PictureUrl = picture,
            };
        }
    }
}
