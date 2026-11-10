using System.Reflection;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Resto.Api.Security;
using Resto.Domain.Platform;
using Resto.Domain.Platform.Abstractions;
using Resto.Infrastructure.Persistence;
using Xunit;

namespace Resto.Api.Tests.Persistence;

public sealed class TenantIsolationTests
{
    [Fact]
    public void BR_PLT_05_EveryMappedTenantEntityUsesBothIdentifiersFromItsOwnContext()
    {
        var first = NewActor();
        var second = NewActor();
        using var firstContext = CreateContext(first);
        using var secondContext = CreateContext(second);

        foreach (var entity in TenantTypes(firstContext))
        {
            var firstSql = QueryRows(firstContext, entity.ClrType).ToQueryString();
            var secondSql = QueryRows(secondContext, entity.ClrType).ToQueryString();
            AssertScopeParameter(entity, firstSql, nameof(TenantEntity.OrganizationId), first.OrganizationId);
            AssertScopeParameter(entity, firstSql, nameof(TenantEntity.RestaurantId), first.RestaurantId);
            AssertScopeParameter(entity, secondSql, nameof(TenantEntity.OrganizationId), second.OrganizationId);
            AssertScopeParameter(entity, secondSql, nameof(TenantEntity.RestaurantId), second.RestaurantId);
        }
    }

    [Theory]
    [InlineData("missing")]
    [InlineData("duplicate tenant claim")]
    [InlineData("multiple authenticated identities")]
    public void BR_PLT_05_InvalidContextCannotQueryAnyTenantEntityOrRoot(string scenario)
    {
        using var db = CreateContext(InvalidActor(scenario));

        foreach (var entity in TenantTypes(db))
        {
            Assert.Throws<InvalidOperationException>(() => QueryRows(db, entity.ClrType).ToQueryString());
        }
        Assert.Throws<InvalidOperationException>(() => db.Organizations.ToQueryString());
        Assert.Throws<InvalidOperationException>(() => db.Restaurants.ToQueryString());
    }

    [Fact]
    public void BR_PLT_05_EveryTenantEntityHasACompositeRestaurantForeignKey()
    {
        using var db = CreateContext(null);
        foreach (var entity in TenantTypes(db))
        {
            var key = Assert.Single(entity.GetForeignKeys(), key => key.PrincipalEntityType.ClrType == typeof(Restaurant));
            Assert.Equal(new[] { nameof(TenantEntity.RestaurantId), nameof(TenantEntity.OrganizationId) },
                key.Properties.Select(property => property.Name));
            Assert.Equal(new[] { nameof(Restaurant.Id), nameof(Restaurant.OrganizationId) },
                key.PrincipalKey.Properties.Select(property => property.Name));
            Assert.True(key.IsRequired);
            Assert.Equal(DeleteBehavior.Restrict, key.DeleteBehavior);

            foreach (var tenantReference in entity.GetForeignKeys()
                         .Where(key => typeof(TenantEntity).IsAssignableFrom(key.PrincipalEntityType.ClrType)))
            {
                AssertScopePair(tenantReference, nameof(TenantEntity.OrganizationId));
                AssertScopePair(tenantReference, nameof(TenantEntity.RestaurantId));
            }
        }
    }

    internal static IEntityType[] TenantTypes(AppDbContext db)
    {
        var types = db.Model.GetEntityTypes()
            .Where(entity => typeof(TenantEntity).IsAssignableFrom(entity.ClrType) && !entity.ClrType.IsAbstract)
            .OrderBy(entity => entity.Name)
            .ToArray();
        Assert.NotEmpty(types);
        return types;
    }

    internal static IQueryable<TenantRow> QueryRows(AppDbContext db, Type entityType, Guid? id = null) =>
        (IQueryable<TenantRow>)QueryMethod.MakeGenericMethod(entityType).Invoke(null, [db, id])!;

    private static readonly MethodInfo QueryMethod = typeof(TenantIsolationTests)
        .GetMethod(nameof(Query), BindingFlags.NonPublic | BindingFlags.Static)!;

    private static IQueryable<TenantRow> Query<TEntity>(AppDbContext db, Guid? id) where TEntity : TenantEntity
    {
        var query = db.Set<TEntity>().AsNoTracking();
        if (id is { } key)
        {
            query = query.Where(row => row.Id == key);
        }
        return query.Select(row => new TenantRow(row.Id, row.OrganizationId, row.RestaurantId));
    }

    internal sealed record TenantRow(Guid Id, Guid OrganizationId, Guid RestaurantId);

    private static void AssertScopeParameter(IEntityType entity, string sql, string property, Guid value)
    {
        var table = StoreObjectIdentifier.Table(entity.GetTableName()!, entity.GetSchema());
        var column = entity.FindProperty(property)!.GetColumnName(table)!;
        var match = Regex.Match(sql, $@"\.""{Regex.Escape(column)}"" = (@\w+)");
        Assert.True(match.Success, $"{entity.DisplayName()} has no SQL equality predicate on {property}.\n{sql}");
        Assert.Contains($"{match.Groups[1].Value}='{value}'", sql);
    }

    private static void AssertScopePair(IForeignKey key, string property)
    {
        Assert.Contains(key.Properties.Zip(key.PrincipalKey.Properties),
            pair => pair.First.Name == property && pair.Second.Name == property);
    }

    private static AppDbContext CreateContext(ICurrentUser? actor) =>
        new(new DbContextOptionsBuilder().UseNpgsql("Host=localhost;Database=resto_model_tests").Options, actor);

    private static AppDbContextTests.TestUser NewActor() =>
        new(Guid.CreateVersion7(), Guid.CreateVersion7(), Guid.CreateVersion7());

    private static ICurrentUser? InvalidActor(string scenario)
    {
        if (scenario == "missing")
        {
            return null;
        }

        var actor = NewActor();
        var identity = new ClaimsIdentity(
            [new("sub", actor.UserId.ToString()), new("org", actor.OrganizationId.ToString()),
                new("rest", actor.RestaurantId.ToString())], "test");
        var principal = new ClaimsPrincipal(identity);
        if (scenario == "duplicate tenant claim")
        {
            identity.AddClaim(new Claim("org", Guid.CreateVersion7().ToString()));
        }
        else
        {
            principal.AddIdentity(new ClaimsIdentity(identity));
        }

        return new HttpCurrentUser(new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext { User = principal }
        });
    }
}
