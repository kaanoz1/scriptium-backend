/*
This file is temporarily disabled. 
Due to budgetary reasons, Scriptium is unable to provide the economic conditions necessary for legal and official procedures involving the collection, processing, and storage of user information.



   namespace ScriptiumBackend.Services;


using MailKit.Security;
using ScriptiumBackend.Interface;
using ScriptiumBackend.Models;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;
public class EmailService(IOptions<EmailSettings> settings) : IEmailService
{
    private readonly EmailSettings _settings = settings.Value;

    public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
    {
        try
        {
            Console.WriteLine($"[INFO] Preparing to send email to {toEmail}");

            MimeMessage email = new MimeMessage();
            email.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            BodyBuilder bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
            email.Body = bodyBuilder.ToMessageBody();

            using var smtp = new SmtpClient();

            Console.WriteLine("[INFO] Connecting to SMTP server...");
            await smtp.ConnectAsync(_settings.SmtpServer, _settings.Port, SecureSocketOptions.SslOnConnect);

            Console.WriteLine("[INFO] Authenticating...");
            await smtp.AuthenticateAsync(_settings.Username, _settings.Password);

            Console.WriteLine("[INFO] Sending email...");
            await smtp.SendAsync(email);
            Console.WriteLine($"[SUCCESS] Email sent to {toEmail}");

            await smtp.DisconnectAsync(true);
        }
        catch (SmtpCommandException ex)
        {
            Console.WriteLine($"[ERROR] SMTP Command Error: {ex.Message} | StatusCode: {ex.StatusCode}");
            throw;
        }
        catch (SmtpProtocolException ex)
        {
            Console.WriteLine($"[ERROR] SMTP Protocol Error: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Unexpected Error: {ex.Message}");
            throw;
        }
    }
}

*/

