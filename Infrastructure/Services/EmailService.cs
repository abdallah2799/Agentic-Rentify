using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Configuration;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var apiKey = _configuration["SendGrid:ApiKey"];
        var client = new SendGridClient(apiKey);
        var fromEmail = _configuration["SendGrid:FromEmail"];
        var fromName = _configuration["SendGrid:FromName"];
        var from = new EmailAddress(fromEmail, fromName);
        var toAddress = new EmailAddress(to);
        var msg = MailHelper.CreateSingleEmail(from, toAddress, subject, body, body);
        var response = await client.SendEmailAsync(msg);
        
        if (!response.IsSuccessStatusCode)
        {
             throw new Exception($"Failed to send email. Status code: {response.StatusCode}");
        }
    }
}
