using CommunityEventManagement.Domain.Interfaces.Patterns;
using CommunityEventManagement.Infrastructure.Patterns.Observer;
using Microsoft.Extensions.Logging;

namespace CommunityEventManagement.Infrastructure.Patterns.Observer.Observers
{
    /// <summary>
    /// Email Notification Observer — Observer in the Observer pattern.
    ///
    /// Observer Pattern (Behavioral):
    ///   - Receives notification when a registration event occurs
    ///   - Independently decides to send an email
    ///   - Subject does not know about email sending logic
    ///
    /// Demonstrates:
    ///   - IEventObserver(T) implementation
    ///   - Single Responsibility: ONLY handles email notifications
    ///   - Stack(T) for notification history (LIFO)
    /// </summary>
    public class EmailNotificationObserver : IEventObserver<RegistrationEventData>
    {
        private readonly ILogger<EmailNotificationObserver> _logger;

        // Stack<T> for notification history — LIFO access
        private readonly Stack<string> _notificationHistory = new();

        public string ObserverName => "EmailNotificationObserver";

        public EmailNotificationObserver(ILogger<EmailNotificationObserver> logger)
        {
            _logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Called by the subject when a registration event occurs.
        /// Sends appropriate email based on the action.
        /// </summary>
        public async Task UpdateAsync(RegistrationEventData eventData)
        {
            if (eventData == null)
                throw new ArgumentNullException(nameof(eventData));

            try
            {
                string emailContent = eventData.Action switch
                {
                    "Registered"  => BuildRegistrationEmail(eventData),
                    "Cancelled"   => BuildCancellationEmail(eventData),
                    "Waitlisted"  => BuildWaitlistEmail(eventData),
                    _             => BuildGenericEmail(eventData)
                };

                await SendEmailAsync(eventData.ParticipantEmail, emailContent);

                // Push to Stack — O(1)
                _notificationHistory.Push(
                    $"{DateTime.Now:HH:mm:ss} | {eventData.Action} | " +
                    $"{eventData.ParticipantEmail} | {eventData.EventName}");

                _logger.LogInformation(
                    "Email notification sent to {Email} for action: {Action}",
                    eventData.ParticipantEmail, eventData.Action);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to send email to {Email}", eventData.ParticipantEmail);
                throw;
            }
        }

        private string BuildRegistrationEmail(RegistrationEventData data) =>
            $"Dear {data.ParticipantName},\n\n" +
            $"Your registration for '{data.EventName}' has been confirmed!\n" +
            $"Event Date: {data.EventStartDate:dddd, dd MMMM yyyy}\n\n" +
            $"Community Events Team";

        private string BuildCancellationEmail(RegistrationEventData data) =>
            $"Dear {data.ParticipantName},\n\n" +
            $"Your registration for '{data.EventName}' has been cancelled.\n\n" +
            $"Community Events Team";

        private string BuildWaitlistEmail(RegistrationEventData data) =>
            $"Dear {data.ParticipantName},\n\n" +
            $"You have been added to the waiting list for '{data.EventName}'.\n\n" +
            $"Community Events Team";

        private string BuildGenericEmail(RegistrationEventData data) =>
            $"Dear {data.ParticipantName},\n\n" +
            $"Your registration status has been updated for '{data.EventName}'.\n\n" +
            $"Community Events Team";

        private async Task SendEmailAsync(string toEmail, string content)
        {
            // Simulated email — in production use SMTP/SendGrid
            await Task.Delay(10);
            _logger.LogDebug(
                "Simulated email sent to: {Email}", toEmail);
        }

        /// <summary>Stack Peek — most recent notification.</summary>
        public string? GetLastNotification()
            => _notificationHistory.Count > 0
                ? _notificationHistory.Peek()
                : null;

        /// <summary>Full notification history in LIFO order.</summary>
        public IEnumerable<string> GetNotificationHistory()
            => _notificationHistory.ToList();

        public int NotificationCount => _notificationHistory.Count;
    }
}