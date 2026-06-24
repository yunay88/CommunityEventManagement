using CommunityEventManagement.Domain.Entities;
using CommunityEventManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CommunityEventManagement.Infrastructure.Data
{
    /// <summary>
    /// Seeds the database with initial data for demonstration.
    /// Called once on application startup if database is empty.
    /// </summary>
    public static class SeedData
    {
        public static async Task InitialiseAsync(ApplicationDbContext context)
        {
            // Run any pending migrations first
            await context.Database.MigrateAsync();

            // Only seed if no venues exist — prevents duplicate seeding
            if (await context.Venues.AnyAsync()) return;

            // ── Seed Venues ───────────────────────────────────────────
            var venues = new List<Venue>
            {
                new Venue("City Community Hall",
                    "123 High Street", "Sunderland", 200),
                new Venue("Riverside Sports Centre",
                    "45 River Road", "Sunderland", 500),
                new Venue("Central Library Meeting Room",
                    "67 Library Lane", "Newcastle", 50),
                new Venue("Innovation Hub",
                    "89 Tech Park", "Durham", 150),
                new Venue("Online Platform",
                    "Virtual", "Remote", 1000)
            };
            await context.Venues.AddRangeAsync(venues);
            await context.SaveChangesAsync();

            // ── Seed Activities ───────────────────────────────────────
            var activities = new List<Activity>
            {
                new Activity("Intro to Coding",
                    "Hands-on beginner programming session",
                    ActivityType.Workshop, 120),
                new Activity("Advanced C# Techniques",
                    "Deep dive into advanced C# features",
                    ActivityType.Workshop, 90),
                new Activity("Tech Trends Talk",
                    "Latest trends in technology for 2024",
                    ActivityType.Talk, 60),
                new Activity("Community Quiz Night",
                    "Fun general knowledge quiz for all ages",
                    ActivityType.Game, 90),
                new Activity("Local Art Exhibition",
                    "Showcase of local artists and sculptures",
                    ActivityType.Exhibition, 180),
                new Activity("Professional Networking",
                    "Meet and connect with local professionals",
                    ActivityType.Networking, 60),
                new Activity("Dance Performance",
                    "Local dance groups showcase their talent",
                    ActivityType.Performance, 45),
                new Activity("Open Mic Night",
                    "Music and spoken word performances",
                    ActivityType.Performance, 120)
            };
            await context.Activities.AddRangeAsync(activities);
            await context.SaveChangesAsync();

            // ── Seed Administrator ────────────────────────────────────
            var admin = new Administrator(
                "Sarah",
                "Johnson",
                "admin@communityevents.com",
                BCryptHash("Admin@1234"),
                "System Administrator",
                "IT");
            await context.Administrators.AddAsync(admin);
            await context.SaveChangesAsync();

            // ── Seed Participants ─────────────────────────────────────
            var participants = new List<Participant>
            {
                new Participant("Alice", "Thompson",
                    "alice@example.com",
                    BCryptHash("Password@1"), "07700111111"),
                new Participant("Bob", "Martinez",
                    "bob@example.com",
                    BCryptHash("Password@1"), "07700222222"),
                new Participant("Carol", "Williams",
                    "carol@example.com",
                    BCryptHash("Password@1"), "07700333333"),
                new Participant("David", "Brown",
                    "david@example.com",
                    BCryptHash("Password@1")),
                new Participant("Emma", "Davis",
                    "emma@example.com",
                    BCryptHash("Password@1"), "07700555555")
            };
            await context.Participants.AddRangeAsync(participants);
            await context.SaveChangesAsync();

            // ── Seed Events ───────────────────────────────────────────
            var now = DateTime.Now;
            var events = new List<Event>
            {
                new Event(
                    "Summer Tech Festival",
                    "Annual technology festival with workshops, talks and networking.",
                    now.AddDays(10),
                    now.AddDays(12),
                    venues[0].Id),
                new Event(
                    "Community Games Day",
                    "A fun day out for all the family with games and activities.",
                    now.AddDays(20),
                    now.AddDays(20).AddHours(8),
                    venues[1].Id),
                new Event(
                    "Winter Networking Evening",
                    "Professional networking event for local business community.",
                    now.AddDays(30),
                    now.AddDays(30).AddHours(3),
                    venues[0].Id),
                new Event(
                    "Art and Culture Week",
                    "Celebrating local art, culture and performance.",
                    now.AddDays(45),
                    now.AddDays(52),
                    venues[2].Id),
                new Event(
                    "Online Learning Workshop",
                    "Remote learning opportunity for all skill levels.",
                    now.AddDays(5),
                    now.AddDays(5).AddHours(2),
                    venues[4].Id),
                new Event(
                    "Past Community Meetup",
                    "A previous community meetup (historical record).",
                    now.AddDays(-30),
                    now.AddDays(-29),
                    venues[0].Id)
            };
            await context.Events.AddRangeAsync(events);
            await context.SaveChangesAsync();

            // ── Seed EventActivities (many-to-many) ───────────────────
            var eventActivities = new List<EventActivity>
            {
                // Tech Festival activities
                new EventActivity
                {
                    EventId = events[0].Id,
                    ActivityId = activities[0].Id,
                    OrderInEvent = 1
                },
                new EventActivity
                {
                    EventId = events[0].Id,
                    ActivityId = activities[2].Id,
                    OrderInEvent = 2
                },
                new EventActivity
                {
                    EventId = events[0].Id,
                    ActivityId = activities[5].Id,
                    OrderInEvent = 3
                },
                // Games Day activities
                new EventActivity
                {
                    EventId = events[1].Id,
                    ActivityId = activities[3].Id,
                    OrderInEvent = 1
                },
                // Networking Evening
                new EventActivity
                {
                    EventId = events[2].Id,
                    ActivityId = activities[5].Id,
                    OrderInEvent = 1
                },
                new EventActivity
                {
                    EventId = events[2].Id,
                    ActivityId = activities[2].Id,
                    OrderInEvent = 2
                },
                // Art Week
                new EventActivity
                {
                    EventId = events[3].Id,
                    ActivityId = activities[4].Id,
                    OrderInEvent = 1
                },
                new EventActivity
                {
                    EventId = events[3].Id,
                    ActivityId = activities[6].Id,
                    OrderInEvent = 2
                },
                // Online Workshop
                new EventActivity
                {
                    EventId = events[4].Id,
                    ActivityId = activities[1].Id,
                    OrderInEvent = 1
                }
            };
            await context.EventActivities.AddRangeAsync(eventActivities);
            await context.SaveChangesAsync();

            // ── Seed Registrations ────────────────────────────────────
            var registrations = new List<Registration>
            {
                new Registration(events[0].Id, participants[0].Id,
                    "Very excited for this!"),
                new Registration(events[0].Id, participants[1].Id),
                new Registration(events[0].Id, participants[2].Id),
                new Registration(events[1].Id, participants[0].Id),
                new Registration(events[1].Id, participants[3].Id),
                new Registration(events[2].Id, participants[4].Id,
                    "Looking forward to networking"),
                new Registration(events[4].Id, participants[1].Id),
                new Registration(events[4].Id, participants[2].Id)
            };

            // Confirm all registrations
            foreach (var reg in registrations)
                reg.Confirm();

            await context.Registrations.AddRangeAsync(registrations);
            await context.SaveChangesAsync();
        }

        // Simple password hashing placeholder
        // In production use BCrypt.Net or ASP.NET Core Identity
        private static string BCryptHash(string password)
        {
            // Simple hash for demo — replace with BCrypt in production
            return Convert.ToBase64String(
                System.Text.Encoding.UTF8.GetBytes(password));
        }
    }
}