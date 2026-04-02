using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace YDeveloper.Services
{
    public class NamecheapService : IDomainService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _clientIp;

        public NamecheapService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _clientIp = _configuration["SystemSettings:ServerIp"] ?? "127.0.0.1";
        }

        // Helper to get Base URL with Auth Params
        private string GetBaseUrl(string command)
        {
            var apiUrl = _configuration["DomainRegistrar:ApiUrl"] ?? "https://api.sandbox.namecheap.com/xml.response";
            var apiUser = _configuration["DomainRegistrar:ApiUser"];
            var apiKey = _configuration["DomainRegistrar:ApiKey"];
            var userName = apiUser; // Usually same

            return $"{apiUrl}?ApiUser={apiUser}&ApiKey={apiKey}&UserName={userName}&ClientIp={_clientIp}&Command={command}";
        }

        public async Task<bool> CheckAvailabilityAsync(string domain)
        {
            var url = $"{GetBaseUrl("namecheap.domains.check")}&DomainList={domain}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) return false;

                var content = await response.Content.ReadAsStringAsync();
                var doc = XDocument.Parse(content);
                if (doc.Root == null) return false;
                var ns = doc.Root.Name.Namespace;

                // Check for Errors
                var errors = doc.Descendants(ns + "Errors").FirstOrDefault();
                if (errors != null && errors.HasElements)
                {
                    Console.WriteLine($"[Namecheap Error]: {errors.Value}");
                    return false;
                }

                // Parse Result
                var result = doc.Descendants(ns + "DomainCheckResult").FirstOrDefault();
                if (result != null)
                {
                    return result.Attribute("Available")?.Value?.ToLower() == "true";
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Namecheap Exception]: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RegisterDomainAsync(string domain, string userId)
        {
            // Requires detailed user info (Address, Phone, etc.)
            // For this simplified version, we'll assume a "Default Contact" profile in appsettings
            // Or use the hardcoded contact info below for the MVP.

            // WARNING: Real registration costs money!
            // Sandbox URL should be used for testing.

            // 1. Construct Registration URL
            var contactParams = GetDefaultContactParams();
            var url = $"{GetBaseUrl("namecheap.domains.create")}&DomainName={domain}&Years=1{contactParams}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                var doc = XDocument.Parse(content);
                if (doc.Root == null) return false;
                var ns = doc.Root.Name.Namespace;

                var errors = doc.Descendants(ns + "Errors").FirstOrDefault();
                if (errors != null && errors.HasElements)
                {
                    Console.WriteLine($"[Namecheap Register Error]: {errors.Value}");
                    return false;
                }

                var result = doc.Descendants(ns + "DomainCreateResult").FirstOrDefault();
                if (result != null && result.Attribute("Registered")?.Value?.ToLower() == "true")
                {
                    // 2. Set Nameservers (DNS) immediately after registration
                    await SetNameserversAsync(domain);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Namecheap Register Exception]: {ex.Message}");
                return false;
            }
        }

        private async Task SetNameserversAsync(string domain)
        {
            // Pointing to DigitalOcean or Cloudflare or Our Own NS
            // Example: ns1.digitalocean.com, ns2.digitalocean.com
            string ns1 = _configuration["DomainRegistrar:Nameserver1"] ?? "dns1.registrar-servers.com";
            string ns2 = _configuration["DomainRegistrar:Nameserver2"] ?? "dns2.registrar-servers.com";

            var url = $"{GetBaseUrl("namecheap.domains.dns.setCustom")}&SLD={domain.Split('.')[0]}&TLD={domain.Split('.')[1]}&Nameservers={ns1},{ns2}";

            await _httpClient.GetAsync(url);
        }

        public async Task<string> GetDomainInfoAsync(string domain)
        {
            var url = $"{GetBaseUrl("namecheap.domains.getinfo")}&DomainName={domain}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                return content; // Returning raw XML for now, logic can be added to parse expiry date.
            }
            catch
            {
                return "Error";
            }
        }

        private string GetDefaultContactParams()
        {
            // Hardcoded contact info for MVP (Namecheap requires this for all contacts: Registrant, Tech, Admin, Aux)
            // In a real app, this comes from the User profile.
            string p = "";
            string[] types = new[] { "Registrant", "Tech", "Admin", "AuxBilling" };

            foreach (var t in types)
            {
                p += $"&{t}FirstName=YDeveloper&{t}LastName=SaaS&{t}Address1=Teknopark&{t}City=Istanbul&{t}StateProvince=Istanbul&{t}PostalCode=34000&{t}Country=TUR&{t}Phone=+90.5555555555&{t}EmailAddress=info@ydeveloper.com";
            }
            return p;
        }
    }
}
