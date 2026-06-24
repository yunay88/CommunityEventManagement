using CommunityEventManagement.Domain.Entities;
using CommunityEventManagement.Domain.Interfaces.Repositories;
using CommunityEventManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CommunityEventManagement.Infrastructure.Repositories
{
    public class VenueRepository : Repository<Venue>, IVenueRepository
    {
        public VenueRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Venue?> GetWithEventsAsync(int id)
        {
            return await _context.Venues
                .Include(v => v.Events)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<IEnumerable<Venue>> GetAvailableVenuesAsync()
        {
            return await _context.Venues
                .Where(v => v.CurrentCapacity > 0)
                .OrderBy(v => v.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        public override async Task<IEnumerable<Venue>> GetAllAsync()
        {
            return await _context.Venues
                .OrderBy(v => v.Name)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}