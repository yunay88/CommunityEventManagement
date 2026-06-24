using CommunityEventManagement.Domain.Entities;
using CommunityEventManagement.Domain.Enums;
using CommunityEventManagement.Domain.Interfaces.Repositories;
using CommunityEventManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CommunityEventManagement.Infrastructure.Repositories
{
    public class ActivityRepository : Repository<Activity>, IActivityRepository
    {
        public ActivityRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Activity>> GetByTypeAsync(ActivityType type)
        {
            return await _context.Activities
                .Where(a => a.Type == type)
                .OrderBy(a => a.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Activity>> GetByEventAsync(int eventId)
        {
            return await _context.Activities
                .Where(a => a.EventActivities
                    .Any(ea => ea.EventId == eventId))
                .OrderBy(a => a.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        public override async Task<IEnumerable<Activity>> GetAllAsync()
        {
            return await _context.Activities
                .OrderBy(a => a.Name)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}