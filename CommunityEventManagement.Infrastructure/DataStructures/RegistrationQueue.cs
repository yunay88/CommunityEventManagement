using CommunityEventManagement.Domain.Entities;

namespace CommunityEventManagement.Infrastructure.DataStructures
{
    /// <summary>
    /// Registration Queue — manages a waiting list for events at capacity.
    /// 
    /// DATA STRUCTURE: Queue(T) — First In, First Out (FIFO)
    /// 
    /// Why Queue for this?
    ///   - Registrations are processed in the ORDER they were received
    ///   - First person to join the waitlist gets the next available spot
    ///   - Queue naturally enforces this FIFO behaviour
    /// 
    /// Time Complexity:
    ///   - Enqueue (add to back): O(1)
    ///   - Dequeue (remove from front): O(1)
    ///   - Peek (view front without removing): O(1)
    /// 
    /// Demonstrates:
    ///   - Queue(T) data structure usage
    ///   - Encapsulation: internal queue not directly accessible
    ///   - Domain-specific behaviour on top of built-in data structure
    /// </summary>
    public class RegistrationQueue
    {
        // The underlying Queue(T) data structure
        private readonly Queue<Registration> _waitingList = new();

        // Event this queue belongs to
        public int EventId { get; }
        public string EventName { get; }

        public RegistrationQueue(int eventId, string eventName)
        {
            if (eventId <= 0)
                throw new ArgumentException("Event ID must be positive.", nameof(eventId));
            if (string.IsNullOrWhiteSpace(eventName))
                throw new ArgumentException("Event name cannot be empty.", nameof(eventName));

            EventId = eventId;
            EventName = eventName;
        }

        /// <summary>
        /// Adds a registration to the back of the waiting list.
        /// Queue operation: Enqueue — O(1)
        /// </summary>
        public void AddToWaitingList(Registration registration)
        {
            if (registration == null)
                throw new ArgumentNullException(nameof(registration));
            if (registration.EventId != EventId)
                throw new InvalidOperationException(
                    $"Registration is for event {registration.EventId}, " +
                    $"not event {EventId}.");

            _waitingList.Enqueue(registration);
        }

        /// <summary>
        /// Removes and returns the next registration from the waiting list.
        /// Queue operation: Dequeue — O(1)
        /// The person who waited longest gets processed first (FIFO).
        /// </summary>
        public Registration? ProcessNext()
        {
            if (_waitingList.Count == 0) return null;
            return _waitingList.Dequeue();
        }

        /// <summary>
        /// Returns the next registration without removing it.
        /// Queue operation: Peek — O(1)
        /// </summary>
        public Registration? PeekNext()
        {
            if (_waitingList.Count == 0) return null;
            return _waitingList.Peek();
        }

        /// <summary>
        /// Checks if a specific participant is in the waiting list.
        /// Note: Queue does not support direct search — O(n) scan required.
        /// This is a trade-off of using Queue over other data structures.
        /// </summary>
        public bool IsParticipantWaiting(int participantId)
        {
            return _waitingList.Any(r => r.ParticipantId == participantId);
        }

        /// <summary>
        /// Returns the position of a participant in the waiting list.
        /// Position 1 = next to be processed.
        /// Returns -1 if not found.
        /// </summary>
        public int GetWaitingPosition(int participantId)
        {
            int position = 1;
            foreach (var registration in _waitingList)
            {
                if (registration.ParticipantId == participantId)
                    return position;
                position++;
            }
            return -1;
        }

        /// <summary>
        /// Processes all waiting registrations and returns them as a list.
        /// Drains the queue completely.
        /// </summary>
        public IEnumerable<Registration> ProcessAll()
        {
            var processed = new List<Registration>();
            while (_waitingList.Count > 0)
            {
                processed.Add(_waitingList.Dequeue());
            }
            return processed;
        }

        // Properties
        public int WaitingCount => _waitingList.Count;
        public bool HasWaiting => _waitingList.Count > 0;

        /// <summary>
        /// Returns a read-only snapshot of the queue in order.
        /// Does not modify the queue.
        /// </summary>
        public IReadOnlyList<Registration> GetWaitingList()
        {
            return _waitingList.ToList().AsReadOnly();
        }

        public override string ToString()
        {
            return $"WaitingList for '{EventName}' | " +
                   $"Waiting: {WaitingCount} | " +
                   $"Next: {PeekNext()?.GetDisplayName() ?? "None"}";
        }
    }
}