using FindTheBug.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace FindTheBug.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _smtpUsername;
    private readonly string _smtpPassword;
    private readonly string _fromEmail;
    private readonly string _fromName;
    private readonly bool _enableSsl;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _smtpHost = configuration["Email:SmtpHost"] ?? "localhost";
        _smtpPort = int.Parse(configuration["Email:SmtpPort"] ?? "587");
        _smtpUsername = configuration["Email:SmtpUsername"] ?? "";
        _smtpPassword = configuration["Email:SmtpPassword"] ?? "";
        _fromEmail = configuration["Email:FromEmail"] ?? "noreply@findthebug.com";
        _fromName = configuration["Email:FromName"] ?? "FindTheBug";
        _enableSsl = bool.Parse(configuration["Email:EnableSsl"] ?? "true");
    }

    public async Task SendPasswordResetEmailAsync(string toEmail, string resetToken, CancellationToken cancellationToken = default)
    {
        var resetUrl = $"{_configuration["AppUrl"]}/reset-password?token={Uri.EscapeDataString(resetToken)}";
        
        var subject = "Password Reset Request";
        var body = $@"
            <html>
            <body>
                <h2>Password Reset Request</h2>
                <p>You have requested to reset your password. Click the link below to reset your password:</p>
                <p><a href='{resetUrl}'>Reset Password</a></p>
                <p>This link will expire in 1 hour.</p>
                <p>If you did not request this, please ignore this email.</p>
                <br/>
                <p>Best regards,<br/>FindTheBug Team</p>
            </body>
            </html>";

        await SendEmailAsync(toEmail, subject, body, cancellationToken);
    }

    public async Task SendWelcomeEmailAsync(string toEmail, string firstName, CancellationToken cancellationToken = default)
    {
        var subject = "Welcome to FindTheBug";
        var body = $@"
            <html>
            <body>
                <h2>Welcome to FindTheBug, {firstName}!</h2>
                <p>Your account has been created successfully.</p>
                <p>You can now log in to access the diagnostics lab management system.</p>
                <br/>
                <p>Best regards,<br/>FindTheBug Team</p>
            </body>
            </html>";

        await SendEmailAsync(toEmail, subject, body, cancellationToken);
    }

    private async Task SendEmailAsync(string toEmail, string subject, string body, CancellationToken cancellationToken)
    {
        try
        {
            using var message = new MailMessage
            {
                From = new MailAddress(_fromEmail, _fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            message.To.Add(toEmail);

            using var smtpClient = new SmtpClient(_smtpHost, _smtpPort)
            {
                EnableSsl = _enableSsl,
                Credentials = new NetworkCredential(_smtpUsername, _smtpPassword)
            };

            await smtpClient.SendMailAsync(message, cancellationToken);
            _logger.LogInformation("Email sent successfully to {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
            // Don't throw - email failures shouldn't break the flow
        }
    }
}
