namespace YDeveloper.Services.Abstractions
{
    public interface IDomainService
    {
        Task<bool> CheckDomainAvailabilityAsync(string domain);
        Task<string> RegisterDomainAsync(string domain, string userId);
        Task<bool> ConfigureDnsAsync(string domain, string ipAddress);
    }

    public interface ISslService
    {
        Task<bool> GenerateCertificateAsync(string domain);
        Task<bool> RenewCertificateAsync(string domain);
        Task<DateTime?> GetExpiryDateAsync(string domain);
    }

    public interface ITemplateService
    {
        Task<List<string>> GetAvailableTemplatesAsync();
        Task<string> GetTemplateContentAsync(string templateId);
        Task<bool> ApplyTemplateAsync(int siteId, string templateId);
    }
}
