using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;

namespace YDeveloper.Services
{
    public class AwsSesEmailService : IEmailService, Microsoft.AspNetCore.Identity.UI.Services.IEmailSender
    {
        private readonly IAmazonSimpleEmailService _sesClient;
        private readonly string _senderEmail;

        public AwsSesEmailService(IAmazonSimpleEmailService sesClient, IConfiguration configuration)
        {
            _sesClient = sesClient;
            _senderEmail = configuration["EmailSettings:SenderEmail"] ?? "info@ydeveloper.com";
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            try
            {
                var sendRequest = new SendEmailRequest
                {
                    Source = _senderEmail,
                    Destination = new Destination
                    {
                        ToAddresses = new List<string> { toEmail }
                    },
                    Message = new Message
                    {
                        Subject = new Content(subject),
                        Body = new Body
                        {
                            Html = new Content
                            {
                                Charset = "UTF-8",
                                Data = message
                            }
                        }
                    }
                };

                var response = await _sesClient.SendEmailAsync(sendRequest);
                Console.WriteLine($"[AWS SES] Email Sent. MessageId: {response.MessageId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AWS SES ERROR] {ex.Message}");
                // In production, you might want to throw or log deeper
            }
        }
    }
}
