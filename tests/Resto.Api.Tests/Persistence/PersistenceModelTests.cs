using Microsoft.EntityFrameworkCore;
using Resto.Domain.Platform;
using Resto.Domain.Platform.Abstractions;
using Resto.Infrastructure.Persistence;
using Xunit;

namespace Resto.Api.Tests.Persistence;

public sealed class PersistenceModelTests
{
    [Fact]
    public void BR_PLT_05_MissingContextCannotGenerateADataQuery()
    {
        using var db = new AppDbContext(Options(), null);
        Assert.Throws<InvalidOperationException>(() => db.Organizations.ToQueryString());
        Assert.Throws<InvalidOperationException>(() => db.Restaurants.ToQueryString());
        Assert.Throws<InvalidOperationException>(() => db.SaveChanges());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void BR_PLT_05_EmptyIdentityIdentifiersFailExplicitly(int missingIndex)
    {
        var ids = new[] { Guid.CreateVersion7(), Guid.CreateVersion7(), Guid.CreateVersion7() };
        ids[missingIndex] = Guid.Empty;
        using var db = new AppDbContext(Options(), new TestUser(ids[0], ids[1], ids[2]));
        Assert.Throws<InvalidOperationException>(() => db.Restaurants.ToQueryString());
    }

    [Fact]
    public void InitialMigrationMatchesRootSchemaAndCurrentModel()
    {
        using var db = new AppDbContext(Options(), null);
        Assert.False(db.Database.HasPendingModelChanges());
        var organization = db.Model.FindEntityType(typeof(Organization))!;
        var restaurant = db.Model.FindEntityType(typeof(Restaurant))!;
        Assert.Null(organization.FindProperty("RestaurantId"));
        Assert.Null(restaurant.FindProperty("RestaurantId"));
        Assert.Equal(typeof(Organization), Assert.Single(restaurant.GetForeignKeys()).PrincipalEntityType.ClrType);
        var sql = db.Database.GenerateCreateScript();
        Assert.Contains("CREATE TABLE organizations", sql);
        Assert.Contains("CREATE TABLE restaurants", sql);
        Assert.Contains("ON DELETE RESTRICT", sql);
    }

    [Fact]
    public void BR_PLT_05_ScopedQueriesTranslateAndCachedModelsUseTheCurrentContext()
    {
        var first = new AppDbContextTests.TestUser(Guid.CreateVersion7(), Guid.CreateVersion7(), Guid.CreateVersion7());
        var second = new AppDbContextTests.TestUser(Guid.CreateVersion7(), Guid.CreateVersion7(), Guid.CreateVersion7());
        using var db = new AppDbContextTests.ProbeContext(Options(), first);
        using var other = new AppDbContextTests.ProbeContext(Options(), second);
        Assert.Contains(first.OrganizationId.ToString(), db.Organizations.ToQueryString());
        Assert.Contains(first.RestaurantId.ToString(), db.Restaurants.ToQueryString());
        var sql = other.Probes.ToQueryString();
        Assert.Contains(second.OrganizationId.ToString(), sql);
        Assert.Contains(second.RestaurantId.ToString(), sql);
        Assert.DoesNotContain(first.OrganizationId.ToString(), sql);
    }

    private static DbContextOptions Options() => new DbContextOptionsBuilder()
        .UseNpgsql("Host=localhost;Database=resto_model_tests").Options;

    private sealed record TestUser(Guid UserId, Guid OrganizationId, Guid RestaurantId) : ICurrentUser
    {
        public Guid? DeviceId => null;
        public bool HasPermission(string permission) => false;
    }
}
