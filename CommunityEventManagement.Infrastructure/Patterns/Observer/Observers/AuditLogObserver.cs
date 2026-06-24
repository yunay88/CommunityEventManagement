using CommunityEventManagement.Domain.Interfaces.Patterns;
using CommunityEventManagement.Infrastructure.Patterns.Observer;
using Microsoft.Extensions.Logging;

namespace CommunityEventManagement.Infrastructure.Patterns.Observer.Observers
{
    /// <summary>
    /// Represents a single audit log entry.
    /// </summary>
    public class AuditLogEntry
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime Timestamp { get; } = DateTime.UtcNow;
        public string Action { get; set; } = string.Empty;
        public int ParticipantId { get; set; }
        public string ParticipantName { get; set; } = string.Empty;
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;

        public override string ToString() =>
            $"[{Timestamp:yyyy-MM-dd HH:mm:ss}] {Action} | " +
            $"Participant: {ParticipantName} ({ParticipantId}) | " +
            $"Event: {EventName} ({EventId})";
    }

    /// <summary>
    /// Audit Log Observer — second Observer implementation.
    ///
    /// Observer Pattern (Behavioral):
    ///   - Completely different behaviour from EmailNotificationObserver
    ///   - Subject does not know about this difference
    ///   - Same IEventObserver(T) interface — polymorphism in action
    ///
    /// Demonstrates:
    ///   - Second IEventObserver implementation (polymorphism)
    ///   - LinkedList(T) for audit log (efficient O(1) append)
    ///   - Single Responsibility: ONLY handles audit logging
    /// </summary>
    public class AuditLogObserver : IEventObserver<RegistrationEventData>
    {
        private readonly ILogger<AuditLogObserver> _logger;

        // LinkedList<T> — efficient O(1) AddLast for audit trail
        private readonly LinkedList<AuditLogEntry> _auditLog = new();
        private const int MaxLogEntries = 1000;

        public string ObserverName => "AuditLogObserver";

        public AuditLogObserver(ILogger<AuditLogObserver> logger)
        {
            _logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Called by subject — creates audit log entry.
        /// Different behaviour from EmailNotificationObserver — polymorphism.
        /// </summary>
        public async Task UpdateAsync(RegistrationEventData eventData)
        {
            if (eventData == null)
                throw new ArgumentNullException(nameof(eventData));

            try
            {
                var entry = new AuditLogEntry
                {
                    Action = eventData.Action,
                    ParticipantId = eventData.ParticipantId,
                    ParticipantName = eventData.ParticipantName,
                    EventId = eventData.EventId,
                    EventName = eventData.EventName,
                    Details =
                        $"Registration ID: {eventData.RegistrationId} | " +
                        $"Occurred: {eventData.OccurredAt:yyyy-MM-dd HH:mm:ss} UTC"
                };

                // LinkedList AddLast — O(1)
                _auditLog.AddLast(entry);

                // Enforce max size — remove oldest if at limit
                if (_auditLog.Count > MaxLogEntries)
                    _auditLog.RemoveFirst();

                _logger.LogInformation(
                    "Audit log entry created: {Entry}", entry.ToString());

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create audit log entry");
                throw;
            }
        }

        /// <summary>
        /// Returns all entries in chronological order.
        /// LinkedList forward traversal using Next.
        /// </summary>
        public IEnumerable<AuditLogEntry> GetAllEntries()
        {
            var node = _auditLog.First;
            while (node != null)
            {
                yield return node.Value;
                node = node.Next;
            }
        }

        /// <summary>
        /// Returns most recent entries using reverse traversal.
        /// LinkedList backward traversal using Previous.
        /// </summary>
        public IEnumerable<AuditLogEntry> GetRecentEntries(int count = 10)
        {
            var result = new List<AuditLogEntry>();
            var node = _auditLog.Last;

            while (node != null && result.Count < count)
            {
                result.Add(node.Value);
                node = node.Previous;
            }

            return result;
        }

        /// <summary>Filters entries by participant ID.</summary>
        public IEnumerable<AuditLogEntry> GetEntriesByParticipant(int participantId)
            => GetAllEntries().Where(e => e.ParticipantId == participantId);

        /// <summary>Filters entries by event ID.</summary>
        public IEnumerable<AuditLogEntry> GetEntriesByEvent(int eventId)
            => GetAllEntries().Where(e => e.EventId == eventId);

        public int TotalEntries => _auditLog.Count;
    }
}