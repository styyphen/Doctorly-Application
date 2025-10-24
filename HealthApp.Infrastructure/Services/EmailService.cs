using HealthApp.Application.Services;
using HealthApp.Infrastructure.Configuration;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace HealthApp.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
    {
        _emailSettings = emailSettings.Value;
        _logger = logger;
    }

    public async Task SendEventCreatedNotificationAsync(string recipientEmail, string recipientName, string eventTitle, DateTime startTime, DateTime endTime)
    {
        var subject = $"New Event Scheduled: {eventTitle}";
        var body = $@"
Dear {recipientName},

A new event has been scheduled:

Event: {eventTitle}
Start Time: {startTime:yyyy-MM-dd HH:mm}
End Time: {endTime:yyyy-MM-dd HH:mm}

Please mark your calendar accordingly.

Best regards,
Healthcare Scheduling System";

        await SendEmailAsync(recipientEmail, subject, body);
    }

    public async Task SendEventUpdatedNotificationAsync(string recipientEmail, string recipientName, string eventTitle, DateTime startTime, DateTime endTime)
    {
        var subject = $"Event Updated: {eventTitle}";
        var body = $@"
Dear {recipientName},

An event has been updated:

Event: {eventTitle}
New Start Time: {startTime:yyyy-MM-dd HH:mm}
New End Time: {endTime:yyyy-MM-dd HH:mm}

Please update your calendar accordingly.

Best regards,
Healthcare Scheduling System";

        await SendEmailAsync(recipientEmail, subject, body);
    }

    public async Task SendEventCancelledNotificationAsync(string recipientEmail, string recipientName, string eventTitle)
    {
        var subject = $"Event Cancelled: {eventTitle}";
        var body = $@"
Dear {recipientName},

The following event has been cancelled:

Event: {eventTitle}

Please remove this event from your calendar.

Best regards,
Healthcare Scheduling System";

        await SendEmailAsync(recipientEmail, subject, body);
    }

    public async Task SendAttendeeInvitationAsync(string recipientEmail, string recipientName, string eventTitle, DateTime startTime, DateTime endTime)
    {
        var subject = $"You're Invited: {eventTitle}";
        var body = $@"
Dear {recipientName},

You have been invited to attend the following event:

Event: {eventTitle}
Start Time: {startTime:yyyy-MM-dd HH:mm}
End Time: {endTime:yyyy-MM-dd HH:mm}

Please confirm your attendance.

Best regards,
Healthcare Scheduling System";

        await SendEmailAsync(recipientEmail, subject, body);
    }

    public async Task SendAttendeeStatusChangedAsync(string organizerEmail, string attendeeName, string eventTitle, string newStatus)
    {
        var subject = $"Attendee Status Update: {eventTitle}";
        var body = $@"
Dear Organizer,

An attendee has updated their status for your event:

Event: {eventTitle}
Attendee: {attendeeName}
New Status: {newStatus}

Best regards,
Healthcare Scheduling System";

        await SendEmailAsync(organizerEmail, subject, body);
    }

    private async Task SendEmailAsync(string recipientEmail, string subject, string body)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
            message.To.Add(new MailboxAddress("", recipientEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                TextBody = body
            };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();

            // For development/testing, we'll just log the email instead of actually sending
            if (string.IsNullOrEmpty(_emailSettings.SmtpHost))
            {
                _logger.LogInformation("Email would be sent to {Email} with subject: {Subject}", recipientEmail, subject);
                _logger.LogInformation("Email body: {Body}", body);
                return;
            }

            await client.ConnectAsync(_emailSettings.SmtpHost, _emailSettings.SmtpPort, _emailSettings.EnableSsl);

            if (!string.IsNullOrEmpty(_emailSettings.Username))
            {
                await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
            }

            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email sent successfully to {Email}", recipientEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", recipientEmail);
            throw;
        }
    }
}