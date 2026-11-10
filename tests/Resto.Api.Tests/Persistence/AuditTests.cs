using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Resto.Domain.Platform;
using Resto.Domain.Platform.Abstractions;
using Resto.Domain.Users;
using Resto.Infrastructure.Persistence;
using Xunit;

namespace Resto.Api.Tests.Persistence;

public sealed class AuditTests
{
    [Fact]
    public async Task BR_PLT_06_AuditServiceRequiresAnAuthenticatedActor()
    {
        await using var db = new AppDbContext(Options(), null);
        var audit = new AuditLog(db);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            audit.WriteAsync(new AuditRecord("Platform", "Update", "Cambió la configuración."), default));
        Assert.Empty(db.ChangeTracker.Entries<AuditEntry>());
    }

    [Fact]
    public void AC_PLT_08_TrackedWritesProduceTenantScopedSnapshotsWithoutCredentials()
    {
        var actor = new AuditActor(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var options = new DbContextOptionsBuilder(Options()).AddInterceptors(new SuppressSave()).Options;
        using var db = new AppDbContext(options, actor);
        db.Restaurants.Add(new Restaurant { Id = actor.RestaurantId, OrganizationId = actor.OrganizationId });
        var user = new User
        {
            OrganizationId = actor.OrganizationId,
            RestaurantId = actor.RestaurantId,
            FullName = "Ana",
            Initials = "A",
            PinHash = "private-pin",
            PasswordHash = "private-password"
        };
        db.Attach(user);
        user.FullName = "Ana Ruiz";

        var start = DateTimeOffset.UtcNow;
        db.SaveChanges();

        var entry = Assert.Single(db.AuditEntries.Local, x => x.EntityId == user.Id);
        Assert.Equal(actor.OrganizationId, entry.OrganizationId);
        Assert.Equal(actor.RestaurantId, entry.RestaurantId);
        Assert.Equal(actor.UserId, entry.UserId);
        Assert.Equal(actor.DeviceId, entry.DeviceId);
        Assert.Equal(TimeSpan.Zero, entry.OccurredAt.Offset);
        Assert.InRange(entry.OccurredAt, start, DateTimeOffset.UtcNow);
        Assert.Equal("Modificó el usuario.", entry.Detail);
        Assert.Contains("\"FullName\":\"Ana\"", entry.BeforeJson);
        Assert.Contains("\"FullName\":\"Ana Ruiz\"", entry.AfterJson);
        Assert.DoesNotContain("private-", entry.BeforeJson);
        Assert.DoesNotContain("private-", entry.AfterJson);
    }

    [Theory]
    [InlineData(EntityState.Modified)]
    [InlineData(EntityState.Deleted)]
    public async Task BR_PLT_06_TrackedAuditEntriesRejectChanges(EntityState state)
    {
        var actor = new AuditActor(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), null);
        await using var db = new AppDbContext(Options(), actor);
        var entry = new AuditEntry { Module = "Platform", Action = "Update", Detail = "Cambió un registro." };
        db.Entry(entry).State = state;

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => db.SaveChangesAsync());
        Assert.Equal("Audit entries are append-only.", exception.Message);
    }

    private static DbContextOptions Options() =>
        new DbContextOptionsBuilder().UseNpgsql("Host=localhost;Database=resto").Options;

    internal sealed record AuditActor(Guid UserId, Guid OrganizationId, Guid RestaurantId, Guid? DeviceId) : ICurrentUser
    {
        public bool HasPermission(string permission) => false;
    }

    private sealed class SuppressSave : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result) =>
            InterceptionResult<int>.SuppressWithResult(0);
    }
}
