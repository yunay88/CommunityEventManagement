using CommunityEventManagement.Domain.Entities;
using CommunityEventManagement.Domain.Interfaces.Repositories;
using CommunityEventManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CommunityEventManagement.Infrastructure.Repositories
{
    public class RegistrationRepository : Repository<Registration>, IRegistrationRepository
    {
        public RegistrationRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Registration>> GetByParticipantAsync(int participantId)
        {
            return await _context.Registrations
                .Include(r => r.Event)
                    .ThenInclude(e => e.Venue)
                .Where(r => r.ParticipantId == participantId)
                .OrderByDescending(r => r.RegistrationDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Registration>> GetByEventAsync(int eventId)
        {
            return await _context.Registrations
                .Include(r => r.Participant)
                .Where(r => r.EventId == eventId)
                .OrderByDescending(r => r.RegistrationDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Registration?> GetWithDetailsAsync(int id)
        {
            return await _context.Registrations
                .Include(r => r.Event)
                    .ThenInclude(e => e.Venue)
                .Include(r => r.Participant)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<bool> ExistsAsync(int participantId, int eventId)
        {
            return await _context.Registrations
                .AnyAsync(r =>
                    r.ParticipantId == participantId &&
                    r.EventId == eventId &&
                    !r.IsDeleted);
        }

        public override async Task<IEnumerable<Registration>> GetAllAsync()
        {
            return await _context.Registrations
                .Include(r => r.Event)
                .Include(r => r.Participant)
                .OrderByDescending(r => r.RegistrationDate)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}