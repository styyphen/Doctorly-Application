namespace HealthApp.Application.Services;

public interface IEmailService
{
    Task SendEventCreatedNotificationAsync(string recipientEmail, string recipientName, string eventTitle, DateTime startTime, DateTime endTime);
    Task SendEventUpdatedNotificationAsync(string recipientEmail, string recipientName, string eventTitle, DateTime startTime, DateTime endTime);
    Task SendEventCancelledNotificationAsync(string recipientEmail, string recipientName, string eventTitle);
    Task SendAttendeeInvitationAsync(string recipientEmail, string recipientName, string eventTitle, DateTime startTime, DateTime endTime);
    Task SendAttendeeStatusChangedAsync(string organizerEmail, string attendeeName, string eventTitle, string newStatus);
}