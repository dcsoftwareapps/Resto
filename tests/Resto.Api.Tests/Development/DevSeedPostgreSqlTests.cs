using Microsoft.EntityFrameworkCore;
using Resto.Api.Development;
using Resto.Api.Tests.Persistence;
using Resto.Domain.Platform;
using Resto.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace Resto.Api.Tests.Development;

public sealed class DevSeedPostgreSqlTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();
    private readonly AppDbContextTests.TestUser _demo = DemoScope();

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        await using var db = Context(_demo);
        await db.Database.MigrateAsync();
    }

    public Task DisposeAsync() => _postgres.DisposeAsync().AsTask();

    [Fact]
    public async Task SeedCreatesTheApprovedRecordsAndAuditsTheDedicatedActor()
    {
        Assert.True(await SeedAsync());
        await using var db = Context(_demo);
        var organization = await db.Organizations.SingleAsync();
        Assert.Equal("Mariscos El Faro", organization.Name);
        Assert.Equal("Mariscos El Faro", organization.LegalName);
        Assert.Equal("Ensenada, B.C.", (await db.Restaurants.SingleAsync()).BranchName);
        Assert.Equal(9, await db.Users.CountAsync());
        Assert.Equal(6, await db.Roles.CountAsync());
        Assert.Equal(93, await db.RolePermissions.CountAsync());
        Assert.False(await db.Users.AnyAsync(user => user.Id == DevSeed.ActorId));
        var audits = await db.AuditEntries.AsNoTracking().ToListAsync();
        Assert.Equal(110, audits.Count);
        Assert.All(audits, entry =>
        {
            Assert.Equal(DevSeed.ActorId, entry.UserId);
            Assert.Equal(_demo.OrganizationId, entry.OrganizationId);
            Assert.Equal(_demo.RestaurantId, entry.RestaurantId);
            Assert.Null(entry.DeviceId);
            Assert.Equal("Create", entry.Action);
        });
    }

    [Fact]
    public async Task RepeatingSeedPreservesEditsAndCreatesNoAdditionalRowsOrAuditEntries()
    {
        Assert.True(await SeedAsync());
        await using var db = Context(_demo);
        var restaurant = await db.Restaurants.SingleAsync();
        restaurant.BranchName = "Sucursal editada";
        await db.SaveChangesAsync();
        var auditCount = await db.AuditEntries.CountAsync();

        Assert.False(await SeedAsync());

        Assert.Equal("Sucursal editada", await db.Restaurants.Select(row => row.BranchName).SingleAsync());
        Assert.Equal(1, await db.Organizations.CountAsync());
        Assert.Equal(1, await db.Restaurants.CountAsync());
        Assert.Equal(9, await db.Users.CountAsync());
        Assert.Equal(6, await db.Roles.CountAsync());
        Assert.Equal(93, await db.RolePermissions.CountAsync());
        Assert.Equal(auditCount, await db.AuditEntries.CountAsync());
    }

    [Fact]
    public async Task SeedLeavesAnExistingTenantUntouchedAndInvisibleToTheDemoScope()
    {
        var other = new AppDbContextTests.TestUser(Guid.CreateVersion7(), Guid.CreateVersion7(), Guid.CreateVersion7());
        await using var db = Context(other);
        await CreateRootsAsync(db, other);

        Assert.True(await SeedAsync());

        Assert.Equal(other.OrganizationId, (await db.Organizations.AsNoTracking().SingleAsync()).Id);
        Assert.Equal(other.RestaurantId, (await db.Restaurants.AsNoTracking().SingleAsync()).Id);
        Assert.Empty(await db.Users.AsNoTracking().ToListAsync());
        Assert.Empty(await db.Roles.AsNoTracking().ToListAsync());
        Assert.Empty(await db.RolePermissions.AsNoTracking().ToListAsync());
        Assert.All(await db.AuditEntries.AsNoTracking().ToListAsync(), entry => Assert.Equal(other.UserId, entry.UserId));

        await using var demo = Context(_demo);
        Assert.Null(await demo.Restaurants.SingleOrDefaultAsync(row => row.Id == other.RestaurantId));
    }

    [Fact]
    public async Task SeedRejectsReservedIdsAlreadyOwnedByExistingData()
    {
        await using var db = Context(_demo);
        await CreateRootsAsync(db, _demo);

        await Assert.ThrowsAsync<InvalidOperationException>(SeedAsync);

        Assert.Equal("Existing", await db.Restaurants.Select(row => row.Name).SingleAsync());
        Assert.Empty(await db.Users.ToListAsync());
        Assert.Equal(2, await db.AuditEntries.CountAsync());
    }

    private Task<bool> SeedAsync() => DevSeed.RunAsync("Development", _postgres.GetConnectionString());

    private AppDbContext Context(AppDbContextTests.TestUser actor) =>
        new(new DbContextOptionsBuilder().UseNpgsql(_postgres.GetConnectionString()).Options, actor);

    private static async Task CreateRootsAsync(AppDbContext db, AppDbContextTests.TestUser actor)
    {
        db.Organizations.Add(new Organization { Id = actor.OrganizationId, Name = "Existing", LegalName = "Existing" });
        db.Restaurants.Add(new Restaurant { Id = actor.RestaurantId, OrganizationId = actor.OrganizationId, Name = "Existing" });
        await db.SaveChangesAsync();
    }

    private static AppDbContextTests.TestUser DemoScope()
    {
        var role = DevSeed.CreateRoster().First();
        return new AppDbContextTests.TestUser(Guid.CreateVersion7(), role.OrganizationId, role.RestaurantId);
    }
}
