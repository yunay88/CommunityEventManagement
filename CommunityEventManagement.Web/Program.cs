using CommunityEventManagement.Domain.Interfaces;
using CommunityEventManagement.Domain.Interfaces.Services;
using CommunityEventManagement.Infrastructure;
using CommunityEventManagement.Infrastructure.Data;
using CommunityEventManagement.Infrastructure.Patterns.Factory;
using CommunityEventManagement.Infrastructure.Patterns.Facade;
using CommunityEventManagement.Infrastructure.Patterns.Observer;
using CommunityEventManagement.Infrastructure.Patterns.Observer.Observers;
using CommunityEventManagement.Infrastructure.Repositories;
using CommunityEventManagement.Infrastructure.Services;
using CommunityEventManagement.Web.Services;
using Microsoft.EntityFrameworkCore;
using CommunityEventManagement.Domain.Interfaces.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ── Add Blazor Server services ─────────────────────────────────────────────
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ── Database — Entity Framework Core with SQLite ───────────────────────────
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=CommunityEvents.db"),
    ServiceLifetime.Scoped);

// ── Repository Pattern — Dependency Injection ──────────────────────────────
// Each repository is Scoped — one instance per HTTP request/circuit
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IParticipantRepository, ParticipantRepository>();
builder.Services.AddScoped<IVenueRepository, VenueRepository>();
builder.Services.AddScoped<IActivityRepository, ActivityRepository>();
builder.Services.AddScoped<IRegistrationRepository, RegistrationRepository>();

// ── Unit of Work ───────────────────────────────────────────────────────────
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ── Service Layer — Business Logic ─────────────────────────────────────────
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IParticipantService, ParticipantService>();
builder.Services.AddScoped<IVenueService, VenueService>();
builder.Services.AddScoped<IActivityService, ActivityService>();
builder.Services.AddScoped<IRegistrationService, RegistrationService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// ── Factory Pattern ────────────────────────────────────────────────────────
builder.Services.AddScoped<EventFactory>();
builder.Services.AddScoped<ParticipantFactory>();

// ── Facade Pattern ─────────────────────────────────────────────────────────
builder.Services.AddScoped<EventManagementFacade>();

// ── Observer Pattern ───────────────────────────────────────────────────────
// Singleton — observers persist for app lifetime
builder.Services.AddSingleton<EmailNotificationObserver>();
builder.Services.AddSingleton<AuditLogObserver>();
builder.Services.AddSingleton<RegistrationNotifier>(provider =>
{
    var logger = provider.GetRequiredService<ILogger<RegistrationNotifier>>();
    var notifier = new RegistrationNotifier(logger);

    // Subscribe both observers at startup
    var emailObserver = provider.GetRequiredService<EmailNotificationObserver>();
    var auditObserver = provider.GetRequiredService<AuditLogObserver>();

    notifier.Subscribe(emailObserver);
    notifier.Subscribe(auditObserver);

    return notifier;
});

// ── Blazor Auth State Service ──────────────────────────────────────────────
// Scoped — one per Blazor circuit (per user)
builder.Services.AddScoped<AuthStateService>();

// ── Logging ────────────────────────────────────────────────────────────────
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});

var app = builder.Build();

// ── Database initialisation — run migrations and seed ─────────────────────
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        await SeedData.InitialiseAsync(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// ── Middleware Pipeline ────────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<CommunityEventManagement.Web.Components.App>()
    .AddInteractiveServerRenderMode();

await app.RunAsync();