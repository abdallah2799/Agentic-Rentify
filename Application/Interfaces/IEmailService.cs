
public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, string otp = "", string actionLink = "", string actionButtonText = "");
}
