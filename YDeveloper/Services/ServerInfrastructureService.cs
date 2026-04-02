using Renci.SshNet;

namespace YDeveloper.Services
{
    public interface IServerInfrastructureService
    {
        Task<bool> ProvisionSiteAsync(string domain, string htmlContent);
    }

    public class ServerInfrastructureService : IServerInfrastructureService
    {
        private readonly IConfiguration _configuration;

        public ServerInfrastructureService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> ProvisionSiteAsync(string domain, string htmlContent)
        {
            var serverIp = _configuration["ServerInfrastructure:IpAddress"];
            var sshUsername = _configuration["ServerInfrastructure:SshUsername"];
            var sshKeyPath = _configuration["ServerInfrastructure:SshKeyPath"];
            var webRoot = _configuration["ServerInfrastructure:WebRootPath"];

            if (serverIp == "YOUR_SERVER_IP")
            {
                Console.WriteLine($"[Mock Provision] Domain: {domain}, Size: {htmlContent.Length} bytes");
                return true; // Mock mode
            }

            try
            {
                if (string.IsNullOrEmpty(sshKeyPath)) throw new ArgumentNullException("SSH Key Path is missing in configuration.");
                var keyFile = new PrivateKeyFile(sshKeyPath);
                var keyFiles = new[] { keyFile };
                var methods = new List<AuthenticationMethod>();
                methods.Add(new PrivateKeyAuthenticationMethod(sshUsername, keyFiles));

                var connectionInfo = new Renci.SshNet.ConnectionInfo(serverIp, sshUsername, methods.ToArray());

                using (var client = new SshClient(connectionInfo))
                {
                    client.Connect();

                    // 1. Create Directory
                    string siteDir = $"{webRoot}/{domain}";
                    client.RunCommand($"mkdir -p {siteDir}");

                    // 2. Upload HTML
                    // Escape single quotes for echo command
                    string safeHtml = htmlContent.Replace("'", "'\\''");
                    client.RunCommand($"echo '{safeHtml}' > {siteDir}/index.html");

                    // 3. Nginx Config (Simplified)
                    string nginxConfig = $@"
server {{
    listen 80;
    server_name {domain} www.{domain};
    root {siteDir};
    index index.html;
}}";
                    string safeConfig = nginxConfig.Replace("'", "'\\''"); // Escape for echo
                    client.RunCommand($"echo '{safeConfig}' | sudo tee /etc/nginx/sites-available/{domain}");
                    client.RunCommand($"sudo ln -s /etc/nginx/sites-available/{domain} /etc/nginx/sites-enabled/");

                    // 4. Reload Nginx
                    client.RunCommand("sudo systemctl reload nginx");

                    client.Disconnect();
                }

                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Provisioning Failed: {ex.Message}");
                return false;
            }
        }
    }
}
