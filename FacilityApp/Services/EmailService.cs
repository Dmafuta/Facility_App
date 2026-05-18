using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace FacilityApp.Services;

public class SmtpSettings
{
    public string Host { get; set; } = "";
    public int Port { get; set; } = 587;
    public bool UseSsl { get; set; } = false;
    public string UserName { get; set; } = "";
    public string Password { get; set; } = "";
    public string FromAddress { get; set; } = "";
    public string FromName { get; set; } = "FacilityApp";
}

public class EmailService : IEmailService
{
    private readonly SmtpSettings _smtp;
    private readonly ILogger<EmailService> _logger;

    public EmailService(SmtpSettings smtp, ILogger<EmailService> logger)
    {
        _smtp   = smtp;
        _logger = logger;
    }

    public async Task SendAsync(string to, string subject, string htmlBody)
    {
        if (string.IsNullOrWhiteSpace(_smtp.Host))
        {
            _logger.LogWarning("SMTP not configured. Skipping email to {To}: {Subject}", to, subject);
            return;
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_smtp.FromName, _smtp.FromAddress));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body    = new TextPart("html") { Text = htmlBody };

        using var client = new SmtpClient();
        await client.ConnectAsync(_smtp.Host, _smtp.Port,
            _smtp.UseSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTlsWhenAvailable);

        if (!string.IsNullOrWhiteSpace(_smtp.UserName))
            await client.AuthenticateAsync(_smtp.UserName, _smtp.Password);

        await client.SendAsync(message);
        await client.DisconnectAsync(true);
        _logger.LogInformation("Email sent to {To}: {Subject}", to, subject);
    }

    public Task SendPasswordResetAsync(string to, string recipientName, string resetLink)
    {
        var html = $"""
            <div style="font-family:Arial,sans-serif;max-width:600px;margin:0 auto;">
              <h2 style="color:#1b6ec2;">Password Reset</h2>
              <p>Hi {HtmlEncode(recipientName)},</p>
              <p>We received a request to reset your password. Click the button below to set a new password.</p>
              <p style="margin:24px 0;">
                <a href="{resetLink}" style="background:#1b6ec2;color:#fff;padding:12px 24px;border-radius:4px;text-decoration:none;font-weight:bold;">
                  Reset Password
                </a>
              </p>
              <p style="color:#666;font-size:14px;">This link expires in 2 hours. If you did not request a password reset, you can safely ignore this email.</p>
              <hr style="border:none;border-top:1px solid #eee;margin:24px 0;" />
              <p style="color:#999;font-size:12px;">FacilityApp — Facility Management System</p>
            </div>
            """;
        return SendAsync(to, "Reset your FacilityApp password", html);
    }

    public Task SendVisitConfirmationAsync(string to, string hostName, string visitorName,
        string purpose, DateTime scheduledAt, string tenantName)
    {
        var when = scheduledAt.ToLocalTime().ToString("dddd, MMMM d 'at' h:mm tt");
        var html = $"""
            <div style="font-family:Arial,sans-serif;max-width:600px;margin:0 auto;">
              <h2 style="color:#1b6ec2;">Visit Confirmation</h2>
              <p>Hi {HtmlEncode(hostName)},</p>
              <p>A visit has been pre-registered for you at <strong>{HtmlEncode(tenantName)}</strong>.</p>
              <table style="width:100%;border-collapse:collapse;margin:16px 0;">
                <tr><td style="padding:8px;border-bottom:1px solid #eee;color:#666;width:130px;">Visitor</td>
                    <td style="padding:8px;border-bottom:1px solid #eee;font-weight:bold;">{HtmlEncode(visitorName)}</td></tr>
                <tr><td style="padding:8px;border-bottom:1px solid #eee;color:#666;">Purpose</td>
                    <td style="padding:8px;border-bottom:1px solid #eee;">{HtmlEncode(purpose)}</td></tr>
                <tr><td style="padding:8px;color:#666;">Scheduled</td>
                    <td style="padding:8px;">{when}</td></tr>
              </table>
              <hr style="border:none;border-top:1px solid #eee;margin:24px 0;" />
              <p style="color:#999;font-size:12px;">{HtmlEncode(tenantName)} — Powered by FacilityApp</p>
            </div>
            """;
        return SendAsync(to, $"Upcoming visit: {visitorName}", html);
    }

    public Task SendCheckInAlertAsync(string to, string hostName, string visitorName,
        string purpose, string tenantName)
    {
        var html = $"""
            <div style="font-family:Arial,sans-serif;max-width:600px;margin:0 auto;">
              <h2 style="color:#198754;">Visitor Checked In</h2>
              <p>Hi {HtmlEncode(hostName)},</p>
              <p>Your visitor <strong>{HtmlEncode(visitorName)}</strong> has just checked in at <strong>{HtmlEncode(tenantName)}</strong>.</p>
              <table style="width:100%;border-collapse:collapse;margin:16px 0;">
                <tr><td style="padding:8px;border-bottom:1px solid #eee;color:#666;width:130px;">Purpose</td>
                    <td style="padding:8px;border-bottom:1px solid #eee;">{HtmlEncode(purpose)}</td></tr>
                <tr><td style="padding:8px;color:#666;">Time</td>
                    <td style="padding:8px;">{DateTime.Now:h:mm tt}</td></tr>
              </table>
              <hr style="border:none;border-top:1px solid #eee;margin:24px 0;" />
              <p style="color:#999;font-size:12px;">{HtmlEncode(tenantName)} — Powered by FacilityApp</p>
            </div>
            """;
        return SendAsync(to, $"Your visitor {visitorName} has arrived", html);
    }

    private static string HtmlEncode(string s) =>
        System.Net.WebUtility.HtmlEncode(s);
}
