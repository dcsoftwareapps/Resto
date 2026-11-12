using Microsoft.AspNetCore.Identity;
using Resto.Api.Development;
using Resto.Contracts;
using Resto.Domain.Users;
using Xunit;

namespace Resto.Api.Tests.Development;

public sealed class DevSeedTests
{
    [Fact]
    public void SeedRequiresTheExactCommand()
    {
        Assert.True(DevSeed.IsRequested(["seed-demo"]));
        Assert.False(DevSeed.IsRequested([]));
        Assert.False(DevSeed.IsRequested(["seed-demo", "--environment", "Development"]));
    }

    [Theory]
    [InlineData("Production")]
    [InlineData("Staging")]
    [InlineData("Unknown")]
    [InlineData("")]
    public async Task SeedRejectsOtherEnvironmentsBeforeAccessingPersistence(string environment)
    {
        var error = await Assert.ThrowsAsync<InvalidOperationException>(() => DevSeed.RunAsync(environment, null));
        Assert.Equal("Demo seeding requires the Development environment.", error.Message);
    }

    [Fact]
    public async Task DevelopmentSeedRequiresAnExplicitDatabaseConnection()
    {
        var error = await Assert.ThrowsAsync<InvalidOperationException>(() => DevSeed.RunAsync("Development", null));
        Assert.Contains("ConnectionStrings__Resto", error.Message);
    }

    [Fact]
    public void RosterUsesStableIdsAndTheApprovedPermissionMatrix()
    {
        var roster = DevSeed.CreateRoster().ToArray();
        Assert.Equal(roster.Select(row => row.Id), DevSeed.CreateRoster().Select(row => row.Id));
        Assert.Equal(roster.Length, roster.Select(row => row.Id).Distinct().Count());
        Assert.Single(roster.Select(row => (row.OrganizationId, row.RestaurantId)).Distinct());

        var roles = roster.OfType<Role>().ToArray();
        Assert.Equal(new[] { "Administrador", "Gerente", "Cajero", "Mesero", "Chef", "Compras" },
            roles.Select(role => role.Name));
        Assert.Equal("Administrador", Assert.Single(roles, role => role.IsSystem).Name);
        var grants = roster.OfType<RolePermission>().ToLookup(grant => grant.RoleId, grant => grant.Permission);
        Assert.Equal(new[] { 37, 33, 8, 2, 9, 4 }, roles.Select(role => grants[role.Id].Count()));
        Assert.True(Permissions.All.SetEquals(grants[roles[0].Id]));
        Assert.Equal(new[] { Permissions.OrdersView, Permissions.PosTake }, grants[roles[3].Id]);
        Assert.Contains(Permissions.InventoryAdjust, grants[roles[4].Id]);
        Assert.DoesNotContain(Permissions.InventoryAdjust, grants[roles[5].Id]);
        Assert.All(grants.SelectMany(grant => grant), permission => Assert.Contains(permission, Permissions.All));

        var users = roster.OfType<User>().ToArray();
        Assert.Equal(9, users.Length);
        Assert.Equal("Brenda Soto", Assert.Single(users, user => !user.IsActive).FullName);
        Assert.DoesNotContain(users, user => user.Id == DevSeed.ActorId);
        Assert.All(users, user =>
        {
            Assert.Contains(roles, role => role.Id == user.RoleId);
            Assert.Null(user.PasswordHash);
        });

        var luis = Assert.Single(users, user => user.FullName == "Luis Ortega");
        Assert.Equal("LO", luis.Initials);
        Assert.Equal(roles[3].Id, luis.RoleId);
        Assert.Equal(PasswordVerificationResult.Success, new PasswordHasher<User>().VerifyHashedPassword(luis, luis.PinHash, "5512"));
    }
}
