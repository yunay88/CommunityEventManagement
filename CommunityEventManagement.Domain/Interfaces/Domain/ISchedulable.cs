namespace CommunityEventManagement.Domain.Interfaces.Domain
{
    /// <summary>
    /// Defines scheduling behaviour for entities that have
    /// a start and end date/time.
    /// Implemented by: Event
    /// Demonstrates: interface defining a contract for behaviour.
    /// </summary>
    public interface ISchedulable
    {
        DateTime StartDate { get; }
        DateTime EndDate { get; }

        /// <summary>Checks if currently active based on current time.</summary>
        bool IsActive();

        /// <summary>
        /// Overloaded version — checks if active at a specific reference date.
        /// Demonstrates: method overloading via interface.
        /// </summary>
        bool IsActive(DateTime referenceDate);

        /// <summary>Returns the total duration of the scheduled item.</summary>
        TimeSpan GetDuration();
    }
}