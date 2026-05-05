using WebApplication1.Entities;

namespace WebApplication1.Interfaces
{
    public interface IProviderRepository
    {
        Task<ExternalLogin?> GetByProviderAndProviderKeyAsync(string name, string key);
        Task<List<ExternalLogin>?> GetByUserIdAsync(Guid UserId);

        Task AddAsync(ExternalLogin externalLogin);
        Task UpdateAsync(ExternalLogin externalLogin);
        Task DeleteAsync(ExternalLogin externalLogin);
    }
}
