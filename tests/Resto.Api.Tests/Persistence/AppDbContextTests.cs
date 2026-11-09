using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Resto.Domain.Platform;
using Resto.Domain.Platform.Abstractions;
using Resto.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace Resto.Api.Tests.Persistence;

public sealed class AppDbContextTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly Guid _restaurantId = Guid.CreateVersion7();
    private readonly Guid _otherRestaurantId = Guid.CreateVersion7();
    private readonly Guid _sameOrganizationRestaurantId = Guid.CreateVersion7();
    private readonly Guid _otherOrganizationId = Guid.CreateVersion7();
    private readonly Guid _userId = Guid.CreateVersion7();

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        await using var db = CreateContext();
        await db.Database.MigrateAsync();
        db.Organizations.Add(new Organization { Id = _organizationId, Name = "One", LegalName = "One" });
        db.Restaurants.Add(new Restaurant { Id = _restaurantId, OrganizationId = _organizationId, Name = "One", BranchName = "One" });
        await db.SaveChangesAsync();
        await using var other = CreateContext(_otherOrganizationId, _otherRestaurantId);
        other.Organizations.Add(new Organization { Id = _otherOrganizationId, Name = "Two", LegalName = "Two" });
        other.Restaurants.Add(new Restaurant { Id = _otherRestaurantId, OrganizationId = _otherOrganizationId, Name = "Two", BranchName = "Two" });
        await other.SaveChangesAsync();
        await using var branch = CreateContext(_organizationId, _sameOrganizationRestaurantId);
        branch.Restaurants.Add(new Restaurant { Id = _sameOrganizationRestaurantId, OrganizationId = _organizationId, Name = "Branch", BranchName = "Branch" });
        await branch.SaveChangesAsync();
        await db.Database.ExecuteSqlRawAsync("CREATE TABLE probes (\"Id\" uuid PRIMARY KEY, \"OrganizationId\" uuid NOT NULL, \"RestaurantId\" uuid NOT NULL, \"CreatedAt\" timestamp with time zone NOT NULL, \"CreatedBy\" uuid NOT NULL, \"UpdatedAt\" timestamp with time zone, \"UpdatedBy\" uuid, \"IsActive\" boolean NOT NULL)");
        await using var probes = CreateProbeContext();
        probes.Probes.Add(new Probe());
        await probes.SaveChangesAsync();
        await using var otherProbes = CreateProbeContext(_otherOrganizationId, _otherRestaurantId);
        otherProbes.Probes.Add(new Probe());
        await otherProbes.SaveChangesAsync();
    }

    public Task DisposeAsync() => _postgres.DisposeAsync().AsTask();

    [Fact]
    public async Task BR_PLT_05_OrganizationsAreRestrictedToTheCurrentOrganization()
    {
        await using var db = CreateContext();
        Assert.Equal(_organizationId, Assert.Single(await db.Organizations.AsNoTracking().ToListAsync()).Id);
        Assert.Null(await db.Organizations.AsNoTracking().SingleOrDefaultAsync(x => x.Id == _otherOrganizationId));
    }

    [Fact]
    public async Task BR_PLT_05_RestaurantsAreRestrictedToBothTenantIdentifiers()
    {
        await using var db = CreateContext();
        Assert.Equal(_restaurantId, Assert.Single(await db.Restaurants.AsNoTracking().ToListAsync()).Id);
        await using var wrong = CreateContext(_organizationId, _otherRestaurantId);
        Assert.Empty(await wrong.Restaurants.AsNoTracking().ToListAsync());
        Assert.Empty(await wrong.Organizations.AsNoTracking().ToListAsync());
        wrong.Restaurants.Add(new Restaurant { Name = "Invalid", BranchName = "Invalid" });
        await Assert.ThrowsAsync<InvalidOperationException>(() => wrong.SaveChangesAsync());
    }

    [Fact]
    public async Task BR_PLT_05_TenantEntityUsesBothIdentifiersAndContextSpecificFilters()
    {
        await using var db = CreateProbeContext();
        var probe = Assert.Single(await db.Probes.AsNoTracking().ToListAsync());
        Assert.Equal(_organizationId, probe.OrganizationId);
        Assert.Equal(_restaurantId, probe.RestaurantId);
        await using var wrong = CreateProbeContext(_organizationId, _otherRestaurantId);
        Assert.Empty(await wrong.Probes.AsNoTracking().ToListAsync());
    }

    [Fact]
    public async Task BR_PLT_05_MissingContextFailsExplicitly()
    {
        await using var db = new AppDbContext(Options(), null);
        await Assert.ThrowsAnyAsync<InvalidOperationException>(() => db.Organizations.ToListAsync());
        db.Organizations.Add(new Organization());
        await Assert.ThrowsAsync<InvalidOperationException>(() => db.SaveChangesAsync());
        await using var invalid = new AppDbContext(Options(), new TestUser(Guid.Empty, _organizationId, _restaurantId));
        await Assert.ThrowsAnyAsync<InvalidOperationException>(() => invalid.Restaurants.ToListAsync());
    }

    [Fact]
    public async Task BR_PLT_05_CrossTenantWritesAndTenantReassignmentAreRejected()
    {
        await using var db = CreateContext();
        var restaurant = await db.Restaurants.SingleAsync();
        restaurant.OrganizationId = _otherOrganizationId;
        await Assert.ThrowsAsync<InvalidOperationException>(() => db.SaveChangesAsync());
        await using var probes = CreateProbeContext();
        probes.Probes.Add(new Probe { OrganizationId = _otherOrganizationId, RestaurantId = _restaurantId });
        await Assert.ThrowsAsync<InvalidOperationException>(() => probes.SaveChangesAsync());
    }

    [Fact]
    public async Task CreationAndModificationMetadataAreGeneratedAndCreationIsPreserved()
    {
        await using var db = CreateProbeContext();
        var probe = new Probe();
        var before = DateTimeOffset.UtcNow;
        db.Probes.Add(probe);
        await db.SaveChangesAsync();
        Assert.InRange(probe.CreatedAt, before, DateTimeOffset.UtcNow);
        Assert.Equal(_userId, probe.CreatedBy);
        Assert.Equal(7, (probe.Id.ToByteArray()[7] >> 4));
        var createdAt = probe.CreatedAt;
        probe.CreatedAt = DateTimeOffset.UnixEpoch;
        probe.CreatedBy = Guid.CreateVersion7();
        probe.IsActive = false;
        db.SaveChanges();
        db.ChangeTracker.Clear();
        var saved = await db.Probes.SingleAsync(x => x.Id == probe.Id);
        Assert.Equal(createdAt, saved.CreatedAt);
        Assert.Equal(_userId, saved.CreatedBy);
        Assert.Equal(_userId, saved.UpdatedBy);
        Assert.NotNull(saved.UpdatedAt);
        Assert.True(saved.UpdatedAt >= saved.CreatedAt);
        var organization = await db.Organizations.SingleAsync();
        var restaurant = await db.Restaurants.SingleAsync();
        Assert.Equal(_userId, organization.CreatedBy);
        Assert.Equal(_userId, restaurant.CreatedBy);
        Assert.True(organization.CreatedAt.Offset == TimeSpan.Zero);
        Assert.True(restaurant.CreatedAt.Offset == TimeSpan.Zero);
    }

    [Fact]
    public async Task RestaurantOrganizationRelationshipIsEnforcedByPostgreSql()
    {
        await using var db = CreateContext();
        var invalidOrganization = Guid.CreateVersion7();
        var invalidRestaurant = Guid.CreateVersion7();
        var error = await Assert.ThrowsAsync<Npgsql.PostgresException>(() => db.Database.ExecuteSqlInterpolatedAsync(
            $"INSERT INTO restaurants (\"Id\", \"OrganizationId\", \"Name\", \"BranchName\", \"CreatedAt\", \"CreatedBy\") VALUES ({invalidRestaurant}, {invalidOrganization}, 'Invalid', 'Invalid', {DateTimeOffset.UtcNow}, {_userId})"));
        Assert.Equal(Npgsql.PostgresErrorCodes.ForeignKeyViolation, error.SqlState);
    }

    [Fact]
    public async Task InitialMigrationMatchesTheModelAndCanBeRolledBack()
    {
        await using var db = CreateContext();
        Assert.False(db.Database.HasPendingModelChanges());
        var migrations = db.Database.GetMigrations().ToArray();
        Assert.EndsWith("_Initial", migrations[0]);
        Assert.Equal(migrations, await db.Database.GetAppliedMigrationsAsync());
        Assert.Equal("America/Tijuana", (await db.Restaurants.SingleAsync()).TimeZone);
        Assert.Equal(new TimeOnly(4, 0), (await db.Restaurants.SingleAsync()).BusinessDayCutoff);
        var migrator = db.GetService<Microsoft.EntityFrameworkCore.Migrations.IMigrator>();
        await db.Database.ExecuteSqlRawAsync("DROP TABLE probes");
        await migrator.MigrateAsync("0");
        Assert.Empty(await db.Database.GetAppliedMigrationsAsync());
        await migrator.MigrateAsync();
    }

    private DbContextOptions Options() => new DbContextOptionsBuilder()
        .UseNpgsql(_postgres.GetConnectionString()).Options;

    private AppDbContext CreateContext(Guid? organization = null, Guid? restaurant = null) =>
        new(Options(), new TestUser(_userId, organization ?? _organizationId, restaurant ?? _restaurantId));

    private ProbeContext CreateProbeContext(Guid? organization = null, Guid? restaurant = null) =>
        new(Options(), new TestUser(_userId, organization ?? _organizationId, restaurant ?? _restaurantId));

    internal sealed record TestUser(Guid UserId, Guid OrganizationId, Guid RestaurantId) : ICurrentUser
    {
        public Guid? DeviceId => null;
        public bool HasPermission(string permission) => false;
    }

    public sealed class Probe : TenantEntity;

    internal sealed class ProbeContext(DbContextOptions options, ICurrentUser user) : AppDbContext(options, user)
    {
        public DbSet<Probe> Probes => Set<Probe>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Probe>().ToTable("probes");
            base.OnModelCreating(modelBuilder);
        }
    }
}
