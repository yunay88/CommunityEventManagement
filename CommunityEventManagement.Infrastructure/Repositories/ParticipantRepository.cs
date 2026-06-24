using CommunityEventManagement.Domain.Entities;
using CommunityEventManagement.Domain.Interfaces.Repositories;
using CommunityEventManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CommunityEventManagement.Infrastructure.Repositories
{
    public class ParticipantRepository : Repository<Participant>, IParticipantRepository
    {
        public ParticipantRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Participant?> GetWithRegistrationsAsync(int id)
        {
            return await _context.Participants
                .Include(p => p.RegistrationsCollection)
                    .ThenInclude(r => r.Event)
                        .ThenInclude(e => e!.Venue)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Participant?> GetByEmailAsync(string email)
        {
            return await _context.Participants
                .AsNoTracking()
                .FirstOrDefaultAsync(p =>
                    p.Email == email.ToLowerInvariant());
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Participants
                .AnyAsync(p => p.Email == email.ToLowerInvariant());
        }

        public override async Task<IEnumerable<Participant>> GetAllAsync()
        {
            return await _context.Participants
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}