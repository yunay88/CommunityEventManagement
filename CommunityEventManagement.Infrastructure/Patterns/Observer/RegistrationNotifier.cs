using CommunityEventManagement.Domain.Interfaces.Patterns;
using Microsoft.Extensions.Logging;

namespace CommunityEventManagement.Infrastructure.Patterns.Observer
{
    /// <summary>
    /// Represents data about a registration event that observers receive.
    /// </summary>
    public class RegistrationEventData
    {
        public int RegistrationId { get; set; }
        public int ParticipantId { get; set; }
        public string ParticipantName { get; set; } = string.Empty;
        public string ParticipantEmail { get; set; } = string.Empty;
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventStartDate { get; set; }
        public string Action { get; set; } = string.Empty;
        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Registration Notifier — Subject in the Observer pattern.
    ///
    /// Observer Pattern (Behavioral):
    ///   - Subject maintains a list of observers
    ///   - When registration changes, notifies ALL subscribed observers
    ///   - Subject does NOT need to know what observers do
    ///   - New observers can be added without changing Subject
    ///
    /// Demonstrates:
    ///   - Observer design pattern
    ///   - ISubject(T) interface implementation
    ///   - Loose coupling between subject and observers
    ///   - List(T) used to maintain observer list
    /// </summary>
    public class RegistrationNotifier : ISubject<RegistrationEventData>
    {
        // List<T> — maintains all subscribed observers
        private readonly List<IEventObserver<RegistrationEventData>> _observers = new();
        private readonly ILogger<RegistrationNotifier> _logger;

        public RegistrationNotifier(ILogger<RegistrationNotifier> logger)
        {
            _logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Subscribes an observer to receive notifications.
        /// List(T).Add — O(1) amortised.
        /// </summary>
        public void Subscribe(IEventObserver<RegistrationEventData> observer)
        {
            if (observer == null)
                throw new ArgumentNullException(nameof(observer));

            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
                _logger.LogInformation(
                    "Observer subscribed: {ObserverName}", observer.ObserverName);
            }
        }

        /// <summary>
        /// Unsubscribes an observer — stops receiving notifications.
        /// List(T).Remove — O(n).
        /// </summary>
        public void Unsubscribe(IEventObserver<RegistrationEventData> observer)
        {
            if (observer == null)
                throw new ArgumentNullException(nameof(observer));

            if (_observers.Remove(observer))
            {
                _logger.LogInformation(
                    "Observer unsubscribed: {ObserverName}", observer.ObserverName);
            }
        }

        /// <summary>
        /// Notifies ALL subscribed observers of a registration event.
        /// Each observer handles the notification independently.
        /// </summary>
        public async Task NotifyObserversAsync(RegistrationEventData eventData)
        {
            if (eventData == null)
                throw new ArgumentNullException(nameof(eventData));

            _logger.LogInformation(
                "Notifying {Count} observer(s) of action: {Action}",
                _observers.Count, eventData.Action);

            foreach (var observer in _observers.ToList())
            {
                try
                {
                    await observer.UpdateAsync(eventData);
                    _logger.LogDebug(
                        "Observer {ObserverName} notified successfully",
                        observer.ObserverName);
                }
                catch (Exception ex)
                {
                    // One observer failing should not break others
                    _logger.LogError(ex,
                        "Observer {ObserverName} threw an exception",
                        observer.ObserverName);
                }
            }
        }

        /// <summary>
        /// Convenience method — notifies observers of a new registration.
        /// </summary>
        public async Task NotifyRegisteredAsync(
            int registrationId,
            int participantId,
            string participantName,
            string participantEmail,
            int eventId,
            string eventName,
            DateTime eventStartDate)
        {
            await NotifyObserversAsync(new RegistrationEventData
            {
                RegistrationId = registrationId,
                ParticipantId = participantId,
                ParticipantName = participantName,
                ParticipantEmail = participantEmail,
                EventId = eventId,
                EventName = eventName,
                EventStartDate = eventStartDate,
                Action = "Registered"
            });
        }

        /// <summary>
        /// Convenience method — notifies observers of a cancellation.
        /// </summary>
        public async Task NotifyCancelledAsync(
            int registrationId,
            int participantId,
            string participantName,
            string participantEmail,
            int eventId,
            string eventName,
            DateTime eventStartDate)
        {
            await NotifyObserversAsync(new RegistrationEventData
            {
                RegistrationId = registrationId,
                ParticipantId = participantId,
                ParticipantName = participantName,
                ParticipantEmail = participantEmail,
                EventId = eventId,
                EventName = eventName,
                EventStartDate = eventStartDate,
                Action = "Cancelled"
            });
        }

        public int ObserverCount => _observers.Count;
    }
}