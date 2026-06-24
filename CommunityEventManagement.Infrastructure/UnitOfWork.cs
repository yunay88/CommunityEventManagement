using CommunityEventManagement.Domain.Interfaces;
using CommunityEventManagement.Domain.Interfaces.Repositories;
using CommunityEventManagement.Infrastructure.Data;
using CommunityEventManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace CommunityEventManagement.Infrastructure
{
    /// <summary>
    /// Unit of Work implementation.
    /// Coordinates all repositories and ensures they share
    /// the same database context and transaction.
    ///
    /// Demonstrates:
    ///   - Unit of Work design pattern
    ///   - IDisposable implementation for resource cleanup
    ///   - Lazy initialisation of repositories
    ///   - Transaction management
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        // Lazy-initialised repositories
        // Only created when first accessed — saves memory
        private IEventRepository? _events;
        private IParticipantRepository? _participants;
        private IVenueRepository? _venues;
        private IActivityRepository? _activities;
        private IRegistrationRepository? _registrations;

        private bool _disposed = false;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context
                ?? throw new ArgumentNullException(nameof(context));
        }

        // Lazy properties — repository created on first access
        public IEventRepository Events =>
            _events ??= new EventRepository(_context);

        public IParticipantRepository Participants =>
            _participants ??= new ParticipantRepository(_context);

        public IVenueRepository Venues =>
            _venues ??= new VenueRepository(_context);

        public IActivityRepository Activities =>
            _activities ??= new ActivityRepository(_context);

        public IRegistrationRepository Registrations =>
            _registrations ??= new RegistrationRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database
                .BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            try
            {
                if (_transaction != null)
                {
                    await _transaction.RollbackAsync();
                }
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        // IDisposable — proper resource cleanup
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _transaction?.Dispose();
                _context.Dispose();
                _disposed = true;
            }
        }
    }
}