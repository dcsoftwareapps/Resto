using Microsoft.EntityFrameworkCore;
using Npgsql;
using Resto.Domain.Platform;
using Resto.Domain.Platform.Abstractions;
using Resto.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace Resto.Api.Tests.Persistence;

public sealed class AuditPostgreSqlTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();
    private readonly AuditTests.AuditActor _actor = new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        await using var db = Context(_actor);
        await db.Database.MigrateAsync();
        await SeedAsync(_actor);
    }

    public Task DisposeAsync() => _postgres.DisposeAsync().AsTask();

    [Fact]
    public async Task AC_PLT_08_AutomaticAndExplicitEntriesPersistWithBeforeAndAfter()
    {
        await using var db = Context(_actor);
        var restaurant = await db.Restaurants.SingleAsync();
        restaurant.Name = "Nuevo nombre";
        await new AuditLog(db).WriteAsync(new AuditRecord("Platform", "Rename", "Cambió el nombre.",
            nameof(Restaurant), restaurant.Id, new { Name = "Original" }, new { restaurant.Name }), default);

        await using var reader = Context(_actor);
        Assert.Equal("Nuevo nombre", (await reader.Restaurants.AsNoTracking().SingleAsync()).Name);
        var entries = await reader.AuditEntries.AsNoTracking()
            .Where(x => x.EntityId == restaurant.Id).ToListAsync();
        var created = Assert.Single(entries, x => x.Action == "Create");
        Assert.Null(created.BeforeJson);
        Assert.Contains("Original", created.AfterJson);
        foreach (var action in new[] { "Rename", "Update" })
        {
            var entry = Assert.Single(entries, x => x.Action == action);
            Assert.Equal(_actor.UserId, entry.UserId);
            Assert.Equal(_actor.DeviceId, entry.DeviceId);
            Assert.Equal(TimeSpan.Zero, entry.OccurredAt.Offset);
            Assert.Contains("Original", entry.BeforeJson);
            Assert.Contains("Nuevo nombre", entry.AfterJson);
        }
    }

    [Fact]
    public async Task BR_PLT_05_AuditEntriesAreIsolatedAcrossOrganizationsAndRestaurants()
    {
        var otherBranch = _actor with { RestaurantId = Guid.NewGuid() };
        var otherTenant = _actor with { OrganizationId = Guid.NewGuid(), RestaurantId = Guid.NewGuid() };
        await SeedAsync(otherBranch, createOrganization: false);
        await SeedAsync(otherTenant);

        await using var db = Context(_actor);
        var entries = await db.AuditEntries.AsNoTracking().ToListAsync();
        Assert.Equal(2, entries.Count);
        Assert.All(entries, entry =>
        {
            Assert.Equal(_actor.OrganizationId, entry.OrganizationId);
            Assert.Equal(_actor.RestaurantId, entry.RestaurantId);
        });
    }

    [Fact]
    public async Task BR_PLT_06_PostgreSqlRejectsBulkUpdatesDeletesAndTruncation()
    {
        await using var db = Context(_actor);
        var update = await Assert.ThrowsAsync<PostgresException>(() =>
            db.AuditEntries.ExecuteUpdateAsync(setters => setters.SetProperty(x => x.Detail, "Cambio")));
        var delete = await Assert.ThrowsAsync<PostgresException>(() => db.AuditEntries.ExecuteDeleteAsync());
        var truncate = await Assert.ThrowsAsync<PostgresException>(() =>
            db.Database.ExecuteSqlRawAsync("TRUNCATE TABLE audit_entries"));
        Assert.All(new[] { update, delete, truncate }, exception => Assert.Equal("55000", exception.SqlState));
        Assert.Equal(2, await db.AuditEntries.CountAsync());
    }

    [Fact]
    public async Task BR_PLT_06_FailedBusinessWriteDoesNotLeaveAnAuditEntry()
    {
        await using var db = Context(_actor);
        db.Restaurants.Add(new Restaurant { Id = _actor.RestaurantId, OrganizationId = _actor.OrganizationId });
        await Assert.ThrowsAsync<DbUpdateException>(() =>
            new AuditLog(db).WriteAsync(new AuditRecord("Platform", "Create", "Creó un restaurante."), default));
        Assert.Empty(db.ChangeTracker.Entries<AuditEntry>());

        await using var reader = Context(_actor);
        Assert.Equal(2, await reader.AuditEntries.CountAsync());
    }

    private async Task SeedAsync(AuditTests.AuditActor actor, bool createOrganization = true)
    {
        await using var db = Context(actor);
        if (createOrganization)
        {
            db.Organizations.Add(new Organization { Id = actor.OrganizationId, Name = "Test", LegalName = "Test" });
        }
        db.Restaurants.Add(new Restaurant { Id = actor.RestaurantId, OrganizationId = actor.OrganizationId, Name = "Original" });
        await db.SaveChangesAsync();
    }

    private AppDbContext Context(AuditTests.AuditActor actor) =>
        new(new DbContextOptionsBuilder().UseNpgsql(_postgres.GetConnectionString()).Options, actor);
}
