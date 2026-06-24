using CommunityEventManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CommunityEventManagement.Infrastructure.Data
{
    /// <summary>
    /// Entity Framework Core Database Context.
    /// Represents a session with the database.
    /// Configures all entity relationships, constraints, and indexes.
    ///
    /// Demonstrates:
    ///   - Repository pattern foundation
    ///   - Database layer of the architecture
    ///   - Fluent API configuration
    ///   - Soft delete global query filters
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        // DbSets — one per entity that maps to a database table
        public DbSet<Event> Events { get; set; } = null!;
        public DbSet<Participant> Participants { get; set; } = null!;
        public DbSet<Administrator> Administrators { get; set; } = null!;
        public DbSet<Venue> Venues { get; set; } = null!;
        public DbSet<Activity> Activities { get; set; } = null!;
        public DbSet<Registration> Registrations { get; set; } = null!;
        public DbSet<EventActivity> EventActivities { get; set; } = null!;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── EventActivity: Composite primary key ──────────────────
            modelBuilder.Entity<EventActivity>()
                .HasKey(ea => new { ea.EventId, ea.ActivityId });

            modelBuilder.Entity<EventActivity>()
                .HasOne(ea => ea.Event)
                .WithMany(e => e.EventActivitiesCollection)
                .HasForeignKey(ea => ea.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EventActivity>()
                .HasOne(ea => ea.Activity)
                .WithMany(a => a.EventActivities)
                .HasForeignKey(ea => ea.ActivityId)
                .OnDelete(DeleteBehavior.Cascade);

            // ── Registration relationships ────────────────────────────
            modelBuilder.Entity<Registration>()
                .HasOne(r => r.Event)
                .WithMany(e => e.RegistrationsCollection)
                .HasForeignKey(r => r.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Registration>()
                .HasOne(r => r.Participant)
                .WithMany(p => p.RegistrationsCollection)
                .HasForeignKey(r => r.ParticipantId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── Unique constraint: one registration per participant per event
            modelBuilder.Entity<Registration>()
                .HasIndex(r => new { r.ParticipantId, r.EventId })
                .IsUnique();

            // ── Event → Venue (optional FK) ───────────────────────────
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Venue)
                .WithMany(v => v.Events)
                .HasForeignKey(e => e.VenueId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            // ── TPH (Table Per Hierarchy) for Person hierarchy ─────────
            // Participant and Administrator share the Participants table
            modelBuilder.Entity<Person>()
                .HasDiscriminator<string>("UserType")
                .HasValue<Participant>("Participant")
                .HasValue<Administrator>("Administrator");

            // ── Soft delete global query filters ──────────────────────
            // These automatically exclude soft-deleted records from ALL queries
            modelBuilder.Entity<Event>()
                .HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Venue>()
                .HasQueryFilter(v => !v.IsDeleted);
            modelBuilder.Entity<Activity>()
                .HasQueryFilter(a => !a.IsDeleted);
            modelBuilder.Entity<Registration>()
                .HasQueryFilter(r => !r.IsDeleted);

            // ── Indexes for query performance ─────────────────────────
            modelBuilder.Entity<Event>()
                .HasIndex(e => e.StartDate)
                .HasDatabaseName("IX_Events_StartDate");

            modelBuilder.Entity<Event>()
                .HasIndex(e => e.VenueId)
                .HasDatabaseName("IX_Events_VenueId");

            modelBuilder.Entity<Participant>()
                .HasIndex(p => p.Email)
                .IsUnique()
                .HasDatabaseName("IX_Participants_Email");

            modelBuilder.Entity<Registration>()
                .HasIndex(r => r.EventId)
                .HasDatabaseName("IX_Registrations_EventId");

            modelBuilder.Entity<Registration>()
                .HasIndex(r => r.ParticipantId)
                .HasDatabaseName("IX_Registrations_ParticipantId");

            // ── Property configurations ───────────────────────────────
            modelBuilder.Entity<Event>()
                .Property(e => e.Name)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Venue>()
                .Property(v => v.Name)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Activity>()
                .Property(a => a.Name)
                .HasMaxLength(100)
                .IsRequired();

            // Store enum as string for readability in database
            modelBuilder.Entity<Registration>()
                .Property(r => r.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Activity>()
                .Property(a => a.Type)
                .HasConversion<string>();

            modelBuilder.Entity<AppUser>()
                .Property(u => u.Role)
                .HasConversion<string>();
        }
    }
}