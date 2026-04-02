using System.Threading.Tasks;

namespace YDeveloper.Services
{
    public interface IDomainService
    {
        Task<bool> CheckAvailabilityAsync(string domain);
        Task<bool> RegisterDomainAsync(string domain, string userId);
        Task<string> GetDomainInfoAsync(string domain);
    }
}
