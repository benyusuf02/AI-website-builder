namespace YDeveloper.Services.Stubs
{
    public class SslServiceStub : Abstractions.ISslService
    {
        public async Task<bool> GenerateCertificateAsync(string domain)
        {
            // TODO: Let's Encrypt integration
            await Task.Delay(100);
            return true;
        }

        public async Task<bool> RenewCertificateAsync(string domain)
        {
            await Task.Delay(100);
            return true;
        }

        public async Task<DateTime?> GetExpiryDateAsync(string domain)
        {
            await Task.Delay(100);
            return DateTime.UtcNow.AddMonths(3);
        }
    }

    public class TemplateServiceStub : Abstractions.ITemplateService
    {
        public async Task<List<string>> GetAvailableTemplatesAsync()
        {
            await Task.CompletedTask;
            return new List<string> { "modern", "classic", "minimal", "corporate" };
        }

        public async Task<string> GetTemplateContentAsync(string templateId)
        {
            await Task.CompletedTask;
            return "<html><body>Template content</body></html>";
        }

        public async Task<bool> ApplyTemplateAsync(int siteId, string templateId)
        {
            await Task.CompletedTask;
            return true;
        }
    }
}
