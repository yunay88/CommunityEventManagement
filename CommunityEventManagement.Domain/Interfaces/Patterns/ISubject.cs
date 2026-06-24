namespace CommunityEventManagement.Domain.Interfaces.Patterns
{
    /// <summary>
    /// Subject interface for the Observer behavioural design pattern.
    /// Implemented by: RegistrationNotifier
    /// </summary>
    public interface ISubject<T>
    {
        /// <summary>Registers an observer to receive notifications.</summary>
        void Subscribe(IEventObserver<T> observer);

        /// <summary>Removes an observer from the notification list.</summary>
        void Unsubscribe(IEventObserver<T> observer);

        /// <summary>Notifies all registered observers of a change.</summary>
        Task NotifyObserversAsync(T eventData);
    }
}