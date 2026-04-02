using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace YDeveloper.Services
{
    public interface IBackupService
    {
        Task TriggerBackupAsync();
    }

    public class BackupService : BackgroundService, IBackupService
    {
        private readonly ILogger<BackupService> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _backupFolder;

        public BackupService(ILogger<BackupService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _backupFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "backups");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Backup Service starting...");

            while (!stoppingToken.IsCancellationRequested)
            {
                // Schedule: Run every 24 hours
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
                await TriggerBackupAsync();
            }
        }

        public async Task TriggerBackupAsync()
        {
            try
            {
                _logger.LogInformation("Starting backup process at: {time}", DateTimeOffset.Now);

                // 1. Ensure backup folder exists
                if (!Directory.Exists(_backupFolder)) Directory.CreateDirectory(_backupFolder);

                // 2. Simulate Backup Creation (e.g., zip wwwroot)
                var backupFileName = $"backup-{DateTime.Now:yyyyMMdd-HHmm}.txt";
                var backupFilePath = Path.Combine(_backupFolder, backupFileName);
                await File.WriteAllTextAsync(backupFilePath, $"Backup created at {DateTime.Now} for YDeveloper.");

                // 3. Upload to S3
                await UploadToS3Async(backupFilePath);

                _logger.LogInformation("Backup completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurring during backup.");
            }
        }

        private async Task UploadToS3Async(string filePath)
        {
            var awsOptions = _configuration.GetSection("AWS");
            var accessKey = awsOptions["AccessKey"];
            var secretKey = awsOptions["SecretKey"];
            var bucketName = awsOptions["BucketName"];
            var region = awsOptions["Region"] ?? "eu-central-1";

            if (string.IsNullOrEmpty(accessKey) || accessKey == "YOUR_AWS_ACCESS_KEY")
            {
                _logger.LogWarning("AWS Credentials not set. Skipping S3 upload.");
                return;
            }

            try
            {
                using var client = new AmazonS3Client(accessKey, secretKey, RegionEndpoint.GetBySystemName(region));
                var fileTransferUtility = new TransferUtility(client);

                await fileTransferUtility.UploadAsync(filePath, bucketName);
                _logger.LogInformation($"Uploaded {Path.GetFileName(filePath)} to S3 bucket {bucketName}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload to S3.");
                throw;
            }
        }
    }
}
