using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using NetWorthTracker.Application.Common.Models;
using NetWorthTracker.Domain.Common.Interfaces;

namespace NetWorthTracker.Application.Common.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
    {
        _emailSettings = emailSettings.Value;
        _logger = logger;
    }

    public void SendPostRegistrationEmail(string username, string toAddress, string confirmationToken)
    {
        var subject = "Welcome to NetWorthTracker!";
        var body = $"Hello {username},\n\nThank you for registering with NetWorthTracker. We're excited to have you on board!\n\nPlease confirm your email by using the following token: {confirmationToken}\n\nBest regards,\nThe NetWorthTracker Team";
        SendEmail(new MailboxAddress(username, toAddress), subject, body);
    }

    private void SendEmail(MailboxAddress to, string subject, string body)
    {
        _logger.LogInformation("Preparing to send email to {ToAddress} with subject '{Subject}'", to.Address, subject);
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("NetWorthTracker", _emailSettings.SmtpUsername));
        message.To.Add(to);
        message.Subject = subject;
        message.Body = new TextPart("plain") { Text = body };

        try
        {
            using var client = new MailKit.Net.Smtp.SmtpClient();

            client.Connect(
                _emailSettings.SmtpServer,
                _emailSettings.SmtpPort,
                SecureSocketOptions.Auto);

            client.Authenticate(
                _emailSettings.SmtpUsername,
                _emailSettings.SmtpPassword);

            var serverResponse = client.Send(message);

            client.Disconnect(true);

            if (serverResponse.StartsWith("OK"))
            {
                _logger.LogInformation("Email accepted by SMTP server.");
            }
            else
            {
                _logger.LogWarning("Email not accepted by SMTP server: {ServerResponse}", serverResponse);
            }
        }
        catch (SmtpCommandException ex)
        {
            _logger.LogError("SMTP command failed: {StatusCode} - {Message}", ex.StatusCode, ex.Message);
        }
        catch (SmtpProtocolException ex)
        {
            _logger.LogError("SMTP protocol failed: {Message}", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while sending email: {Message}", ex.Message);
        }
    }
}
