using Microsoft.EntityFrameworkCore;
using Npgsql;
using Resto.Contracts;
using Resto.Domain.Platform;
using Resto.Domain.Users;
using Resto.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace Resto.Api.Tests.Persistence;

public sealed class UsersAndRolesTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly Guid _firstRestaurant = Guid.CreateVersion7();
    private readonly Guid _secondRestaurant = Guid.CreateVersion7();
    private readonly Guid _otherOrganization = Guid.CreateVersion7();
    private readonly Guid _otherRestaurant = Guid.CreateVersion7();
    private readonly Guid _actor = Guid.CreateVersion7();
    private readonly Role _firstRole;
    private readonly Role _secondRole;
    private readonly Role _otherRole;

    public UsersAndRolesTests()
    {
        _firstRole = new Role(_organizationId, _firstRestaurant) { Name = "First" };
        _secondRole = new Role(_organizationId, _secondRestaurant) { Name = "Second" };
        _otherRole = new Role(_otherOrganization, _otherRestaurant) { Name = "Other" };
    }

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        await using var db = Context(_organizationId, _firstRestaurant);
        await db.Database.MigrateAsync();
        await SeedAsync(_organizationId, _firstRestaurant, _firstRole, createOrganization: true);
        await SeedAsync(_organizationId, _secondRestaurant, _secondRole, createOrganization: false);
        await SeedAsync(_otherOrganization, _otherRestaurant, _otherRole, createOrganization: true);
    }

    public Task DisposeAsync() => _postgres.DisposeAsync().AsTask();

    [Fact]
    public async Task BR_PLT_11_ClockFactoryUsesOnlyTheAuthorizedRestaurantSettings()
    {
        await using var first = Context(_organizationId, _firstRestaurant);
        await using var second = Context(_organizationId, _secondRestaurant);
        await using var mismatched = Context(_otherOrganization, _firstRestaurant);
        var secondRestaurant = await second.Restaurants.SingleAsync();
        secondRestaurant.TimeZone = "Asia/Tokyo";
        secondRestaurant.BusinessDayCutoff = new TimeOnly(6, 0);
        await second.SaveChangesAsync();

        var firstClock = await new RestaurantClockFactory(first, TimeProvider.System).CreateAsync();
        var secondClock = await new RestaurantClockFactory(second, TimeProvider.System).CreateAsync();
        var instant = new DateTimeOffset(2026, 1, 2, 10, 0, 0, TimeSpan.Zero);
        Assert.Equal(new DateOnly(2026, 1, 1), firstClock.BusinessDay(instant));
        Assert.Equal(new DateOnly(2026, 1, 2), secondClock.BusinessDay(instant));
        Assert.Equal(new DateOnly(2026, 1, 2), secondClock.BusinessDay(instant.AddHours(10)));
        Assert.Equal(new DateOnly(2026, 1, 3), secondClock.BusinessDay(instant.AddHours(11)));
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            new RestaurantClockFactory(mismatched, TimeProvider.System).CreateAsync());
    }

    [Fact]
    public async Task BR_PLT_05_AC_PLT_07_EveryMappedTenantEntityHidesOtherTenantRows()
    {
        await using var first = Context(_organizationId, _firstRestaurant);
        await using var sibling = Context(_organizationId, _secondRestaurant);
        await using var foreign = Context(_otherOrganization, _otherRestaurant);
        await using var mismatched = Context(_otherOrganization, _firstRestaurant);

        foreach (var entity in TenantIsolationTests.TenantTypes(first))
        {
            var type = entity.ClrType;
            var own = await TenantIsolationTests.QueryRows(first, type).ToListAsync();
            var siblings = await TenantIsolationTests.QueryRows(sibling, type).ToListAsync();
            var foreignRows = await TenantIsolationTests.QueryRows(foreign, type).ToListAsync();

            AssertScope(own, entity.Name, _organizationId, _firstRestaurant);
            AssertScope(siblings, entity.Name, _organizationId, _secondRestaurant);
            AssertScope(foreignRows, entity.Name, _otherOrganization, _otherRestaurant);
            Assert.Empty(await TenantIsolationTests.QueryRows(mismatched, type).ToListAsync());

            Assert.NotNull(await TenantIsolationTests.QueryRows(first, type, own[0].Id).SingleOrDefaultAsync());
            foreach (var row in siblings.Concat(foreignRows))
            {
                Assert.Null(await TenantIsolationTests.QueryRows(first, type, row.Id).SingleOrDefaultAsync());
            }
        }
    }

    private static void AssertScope(
        IReadOnlyCollection<TenantIsolationTests.TenantRow> rows, string entity, Guid organization, Guid restaurant)
    {
        Assert.True(rows.Count > 0, $"Seed {entity} in all three scopes before validating tenant isolation.");
        Assert.All(rows, row =>
        {
            Assert.Equal(organization, row.OrganizationId);
            Assert.Equal(restaurant, row.RestaurantId);
        });
    }

    [Fact]
    public async Task BR_PLT_05_UsersRolesAndGrantsAreScopedToTheRestaurant()
    {
        await using var db = Context(_organizationId, _firstRestaurant);
        var user = Assert.Single(await db.Users.AsNoTracking().ToListAsync());
        Assert.Equal(_firstRole.Id, user.RoleId);
        Assert.Equal(_firstRole.Id, Assert.Single(await db.Roles.AsNoTracking().ToListAsync()).Id);
        Assert.Equal(_firstRole.Id, Assert.Single(await db.RolePermissions.AsNoTracking().ToListAsync()).RoleId);

        await using var invalid = Context(_otherOrganization, _firstRestaurant);
        Assert.Empty(await invalid.Users.AsNoTracking().ToListAsync());
        Assert.Empty(await invalid.Roles.AsNoTracking().ToListAsync());
        Assert.Empty(await invalid.RolePermissions.AsNoTracking().ToListAsync());
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task BR_PLT_05_DatabaseRejectsRoleReferencesOutsideTheScope(bool permissionGrant)
    {
        await using var db = Context(_organizationId, _firstRestaurant);
        if (permissionGrant)
        {
            db.RolePermissions.Add(new RolePermission { RoleId = _otherRole.Id, Permission = Permissions.InventoryAdjust });
        }
        else
        {
            db.Users.Add(NewUser(_secondRole.Id));
        }

        var exception = await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        Assert.Equal(PostgresErrorCodes.ForeignKeyViolation, Assert.IsType<PostgresException>(exception.InnerException).SqlState);
    }

    [Fact]
    public async Task DatabaseRejectsDuplicateGrantsForARole()
    {
        await using var db = Context(_organizationId, _firstRestaurant);
        db.RolePermissions.Add(new RolePermission { RoleId = _firstRole.Id, Permission = Permissions.InventoryView });

        var exception = await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        Assert.Equal(PostgresErrorCodes.UniqueViolation, Assert.IsType<PostgresException>(exception.InnerException).SqlState);
    }

    private async Task SeedAsync(Guid organization, Guid restaurant, Role role, bool createOrganization)
    {
        await using var db = Context(organization, restaurant);
        if (createOrganization)
        {
            db.Organizations.Add(new Organization { Id = organization, Name = "Test", LegalName = "Test" });
        }
        db.Restaurants.Add(new Restaurant { Id = restaurant, OrganizationId = organization, Name = "Test", BranchName = role.Name });
        db.Roles.Add(role);
        db.Users.Add(NewUser(role.Id));
        db.RolePermissions.Add(new RolePermission { RoleId = role.Id, Permission = Permissions.InventoryView });
        await db.SaveChangesAsync();
    }

    private AppDbContext Context(Guid organization, Guid restaurant) =>
        new(new DbContextOptionsBuilder().UseNpgsql(_postgres.GetConnectionString()).Options,
            new AppDbContextTests.TestUser(_actor, organization, restaurant));

    private static User NewUser(Guid roleId) => new()
    {
        FullName = "Test user",
        Initials = "TU",
        RoleId = roleId,
        PinHash = "test-hash"
    };
}
