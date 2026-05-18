namespace FacilityApp.Services;

public interface IEmailService
{
    Task SendAsync(string to, string subject, string htmlBody);
    Task SendPasswordResetAsync(string to, string recipientName, string resetLink);
    Task SendVisitConfirmationAsync(string to, string hostName, string visitorName,
        string purpose, DateTime scheduledAt, string tenantName);
    Task SendCheckInAlertAsync(string to, string hostName, string visitorName,
        string purpose, string tenantName);
}
