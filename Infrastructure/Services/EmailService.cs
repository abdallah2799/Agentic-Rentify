using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Configuration;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly EmailTemplateService _emailTemplateService;

    public EmailService(IConfiguration configuration, EmailTemplateService emailTemplateService)
    {
        _configuration = configuration;
        _emailTemplateService = emailTemplateService;
    }

    public async Task SendEmailAsync(string to, string subject, string body, string otp = "", string actionLink = "", string actionButtonText = "")
    {
        var apiKey = _configuration["SendGrid:ApiKey"];
        var client = new SendGridClient(apiKey);
        var fromEmail = _configuration["SendGrid:FromEmail"];
        var fromName = _configuration["SendGrid:FromName"];
        var from = new EmailAddress(fromEmail, fromName);
        var toAddress = new EmailAddress(to);

        // Fetch Logo URL (should be in config or Cloudinary) 
        // For now, defaulting or hardcoding as per user preference context
        string logoUrl = "https://res.cloudinary.com/dijyfu0m3/image/upload/v1766090397/zocvo7jehb3elemrwv2x.png"; 

        // Generate the templated body using the "body" argument as the "BODY_TEXT"
        // We leave OTP_CODE and others empty if not provided in the body string (which is a bit hacky, but requested: "all emails usage template")
        // Better approach: Check if body is plain text, then wrap it.
        
        string templatedBody = _emailTemplateService.GenerateEmailBody(
            heading: subject, 
            bodyText: body, 
            otpCode: otp,
            actionLink: actionLink,
            actionButtonText: actionButtonText,
            userEmail: to, 
            logoUrl: logoUrl
        );

        var msg = MailHelper.CreateSingleEmail(from, toAddress, subject, templatedBody, templatedBody);
        var response = await client.SendEmailAsync(msg);
        
        if (!response.IsSuccessStatusCode)
        {
             throw new Exception($"Failed to send email. Status code: {response.StatusCode}");
        }
    }
}
