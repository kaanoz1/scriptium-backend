namespace scriptium_backend.Interface;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string htmlBody);
}
