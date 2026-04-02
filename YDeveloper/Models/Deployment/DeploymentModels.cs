namespace YDeveloper.Models.Deployment
{
    public class DeploymentConfig
    {
        public string Environment { get; set; } = "Production";
        public string ServerUrl { get; set; } = string.Empty;
        public Dictionary<string, string> EnvironmentVariables { get; set; } = new();
        public bool AutoDeploy { get; set; }
    }

    public class DeploymentLog
    {
        public int Id { get; set; }
        public int SiteId { get; set; }
        public string Version { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public DateTime DeployedAt { get; set; } = DateTime.UtcNow;
        public string? ErrorMessage { get; set; }
    }
}
