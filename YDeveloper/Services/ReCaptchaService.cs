using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace YDeveloper.Services
{
    public interface IReCaptchaService
    {
        Task<bool> VerifyAsync(string token);
    }

    public class ReCaptchaService : IReCaptchaService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly ILogger<ReCaptchaService> _logger;

        public ReCaptchaService(IConfiguration configuration, HttpClient httpClient, ILogger<ReCaptchaService> logger)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<bool> VerifyAsync(string token)
        {
            var secretKey = _configuration["ReCaptcha:SecretKey"];

            // BYPASS FOR GOOGLE TEST KEYS
            // The standard test secret key is 6LeIxAcTAAAAAGG-vFI1TnRWxMZNFuojJ4WifJWe
            if (secretKey == "6LeIxAcTAAAAAGG-vFI1TnRWxMZNFuojJ4WifJWe")
            {
                _logger.LogInformation("ReCaptcha: Test Key detected, skipping verification.");
                return true;
            }

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("ReCaptcha token is missing.");
                return false;
            }

            try
            {
                var response = await _httpClient.PostAsync($"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={token}", null);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ReCaptchaResponse>();
                    if (result != null && result.Success && result.Score >= 0.5)
                    {
                        return true;
                    }
                    _logger.LogWarning($"ReCaptcha verification failed. Success: {result?.Success}, Score: {result?.Score}, Errors: {string.Join(",", result?.ErrorCodes ?? new string[0])}");
                }
                else
                {
                    _logger.LogError($"ReCaptcha API error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying ReCaptcha token.");
            }

            return false;
        }

        private class ReCaptchaResponse
        {
            [JsonPropertyName("success")]
            public bool Success { get; set; }

            [JsonPropertyName("score")]
            public double Score { get; set; }

            [JsonPropertyName("action")]
            public string? Action { get; set; }

            [JsonPropertyName("challenge_ts")]
            public DateTime ChallengeTs { get; set; }

            [JsonPropertyName("hostname")]
            public string? Hostname { get; set; }

            [JsonPropertyName("error-codes")]
            public string[]? ErrorCodes { get; set; }
        }
    }
}
