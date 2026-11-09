using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Resto.Api.Security;
using Resto.Contracts;
using Resto.Domain.Platform.Abstractions;
using Resto.Infrastructure.Persistence;
using Xunit;

namespace Resto.Api.Tests.Security;

public sealed class HttpCurrentUserTests
{
    [Fact]
    public void ApiRegistersScopedIdentityAndPersistenceRejectsAnAnonymousQuery()
    {
        using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder.UseSetting("ConnectionStrings:Resto", "Host=localhost;Database=resto"));
        using var scope = factory.Services.CreateScope();
        var services = scope.ServiceProvider;
        var current = Assert.IsType<HttpCurrentUser>(services.GetRequiredService<ICurrentUser>());
        Assert.Same(current, services.GetRequiredService<ICurrentUser>());
        AssertDenied(current);

        var db = services.GetRequiredService<AppDbContext>();
        Assert.Throws<InvalidOperationException>(() => db.Users.ToQueryString());
    }

    [Fact]
    public void ReadsAuthenticatedScopeAndGrantsOnlyExactCatalogPermissions()
    {
        var claims = ScopeClaims();
        var deviceId = Guid.NewGuid();
        var current = CurrentUser(new ClaimsIdentity(
            [.. claims, new("dev", deviceId.ToString()), new("perm", Permissions.InventoryView),
                new("perm", "inventory.*"), new("perm", "unknown.permission")], "test"));

        Assert.Equal(Guid.Parse(claims[0].Value), current.UserId);
        Assert.Equal(Guid.Parse(claims[1].Value), current.OrganizationId);
        Assert.Equal(Guid.Parse(claims[2].Value), current.RestaurantId);
        Assert.Equal(deviceId, current.DeviceId);
        Assert.True(current.HasPermission(Permissions.InventoryView));
        Assert.False(current.HasPermission(Permissions.InventoryAdjust));
        Assert.False(current.HasPermission("unknown.permission"));
    }

    [Fact]
    public void AuthenticatedUserMayHaveNoDevice()
    {
        var current = CurrentUser(new ClaimsIdentity(ScopeClaims(), "test"));
        Assert.Null(current.DeviceId);
    }

    [Fact]
    public void AnonymousClaimsAndMissingHttpContextCannotAuthorize()
    {
        var anonymous = CurrentUser(new ClaimsIdentity([.. ScopeClaims(), new("perm", Permissions.InventoryView)]));
        AssertDenied(anonymous);
        AssertDenied(new HttpCurrentUser(new HttpContextAccessor()));
    }

    [Theory]
    [InlineData("sub", "missing")]
    [InlineData("org", "missing")]
    [InlineData("rest", "missing")]
    [InlineData("org", "duplicate")]
    [InlineData("rest", "empty")]
    [InlineData("dev", "invalid")]
    public void InvalidScopeFailsClosed(string claimType, string failure)
    {
        var claims = ScopeClaims().ToList();
        if (failure != "duplicate")
        {
            claims.RemoveAll(claim => claim.Type == claimType);
        }
        if (failure != "missing")
        {
            var value = failure == "empty" ? Guid.Empty.ToString() : "invalid";
            claims.Add(new Claim(claimType, value));
        }
        claims.Add(new Claim("perm", Permissions.InventoryView));

        AssertDenied(CurrentUser(new ClaimsIdentity(claims, "test")));
    }

    [Fact]
    public void PermissionsFromOtherIdentitiesAreExcluded()
    {
        var scoped = new ClaimsIdentity(ScopeClaims(), "test");
        var permission = new ClaimsIdentity([new Claim("perm", Permissions.InventoryView)]);
        Assert.False(CurrentUser(scoped, permission).HasPermission(Permissions.InventoryView));

        var secondAuthenticatedIdentity = new ClaimsIdentity(ScopeClaims(), "second");
        AssertDenied(CurrentUser(scoped, secondAuthenticatedIdentity));
    }

    private static Claim[] ScopeClaims() =>
        [new("sub", Guid.NewGuid().ToString()), new("org", Guid.NewGuid().ToString()),
            new("rest", Guid.NewGuid().ToString())];

    private static HttpCurrentUser CurrentUser(params ClaimsIdentity[] identities) =>
        new(new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identities) }
        });

    private static void AssertDenied(HttpCurrentUser current)
    {
        Assert.False(current.HasPermission(Permissions.InventoryView));
        Assert.Throws<InvalidOperationException>(() => current.UserId);
        Assert.Throws<InvalidOperationException>(() => current.OrganizationId);
        Assert.Throws<InvalidOperationException>(() => current.RestaurantId);
    }
}
