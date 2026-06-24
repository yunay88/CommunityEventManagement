namespace CommunityEventManagement.Domain.Interfaces.Patterns
{
    /// <summary>
    /// Custom observer interface for the Observer behavioural design pattern.
    /// Named IEventObserver to avoid conflict with System.IObserver(T).
    /// Implemented by: EmailNotificationObserver, AuditLogObserver
    /// </summary>
    public interface IEventObserver<T>
    {
        /// <summary>
        /// Called by the subject when a change occurs.
        /// </summary>
        Task UpdateAsync(T eventData);

        /// <summary>
        /// Human-readable name of this observer, used for logging.
        /// </summary>
        string ObserverName { get; }
    }
}