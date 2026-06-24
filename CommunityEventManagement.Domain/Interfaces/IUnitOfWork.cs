using CommunityEventManagement.Domain.Interfaces.Repositories;

namespace CommunityEventManagement.Domain.Interfaces
{
    /// <summary>
    /// Unit of Work pattern interface.
    /// Coordinates multiple repository operations as a single transaction.
    /// Ensures all changes are saved together or rolled back together.
    /// Demonstrates:
    ///   - Unit of Work design pattern
    ///   - IDisposable for proper resource cleanup
    ///   - Aggregating multiple interfaces into one
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        IEventRepository Events { get; }
        IParticipantRepository Participants { get; }
        IVenueRepository Venues { get; }
        IActivityRepository Activities { get; }
        IRegistrationRepository Registrations { get; }

        /// <summary>Saves all pending changes to the database.</summary>
        Task<int> SaveChangesAsync();

        /// <summary>Begins a database transaction.</summary>
        Task BeginTransactionAsync();

        /// <summary>Commits the current transaction.</summary>
        Task CommitTransactionAsync();

        /// <summary>Rolls back the current transaction on failure.</summary>
        Task RollbackTransactionAsync();
    }
}