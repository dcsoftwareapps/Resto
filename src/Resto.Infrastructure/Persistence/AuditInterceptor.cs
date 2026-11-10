using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Resto.Domain.Platform;
using Resto.Domain.Platform.Abstractions;

namespace Resto.Infrastructure.Persistence;

internal sealed class AuditInterceptor : SaveChangesInterceptor
{
    private readonly List<AuditEntry> _generated = [];

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        Prepare((AppDbContext)eventData.Context!);
        return result;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Prepare((AppDbContext)eventData.Context!);
        return ValueTask.FromResult(result);
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        _generated.Clear();
        return result;
    }

    public override ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default) =>
        ValueTask.FromResult(SavedChanges(eventData, result));

    public override void SaveChangesFailed(DbContextErrorEventData eventData) => Discard(eventData.Context!);

    public override Task SaveChangesFailedAsync(
        DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
    {
        SaveChangesFailed(eventData);
        return Task.CompletedTask;
    }

    public override void SaveChangesCanceled(DbContextEventData eventData) => Discard(eventData.Context!);

    public override Task SaveChangesCanceledAsync(
        DbContextEventData eventData, CancellationToken cancellationToken = default)
    {
        SaveChangesCanceled(eventData);
        return Task.CompletedTask;
    }

    private void Discard(DbContext context)
    {
        foreach (var entry in _generated)
        {
            context.Entry(entry).State = EntityState.Detached;
        }
        _generated.Clear();
    }

    private void Prepare(AppDbContext context)
    {
        // A later interceptor can reject the save before EF calls failure handlers.
        Discard(context);

        foreach (var entry in context.ChangeTracker.Entries<AuditEntry>())
        {
            if (entry.State is EntityState.Modified or EntityState.Deleted)
            {
                throw new InvalidOperationException("Audit entries are append-only.");
            }
            if (entry.State == EntityState.Added)
            {
                var actor = context.RequireCurrentUser();
                if (entry.Entity.UserId != actor.UserId || entry.Entity.DeviceId != actor.DeviceId)
                {
                    throw new InvalidOperationException("Audit actor must match the authenticated user and device.");
                }
            }
        }

        var auditEntries = context.ChangeTracker.Entries<Entity>()
            .Where(entry => entry.State is EntityState.Added or EntityState.Modified)
            .Select(CreateRecord)
            .OfType<AuditRecord>()
            .Select(record => AuditLog.CreateEntry(context, record))
            .ToList();

        _generated.AddRange(auditEntries);
        context.AuditEntries.AddRange(auditEntries);
    }

    private static AuditRecord? CreateRecord(EntityEntry<Entity> entry)
    {
        var audited = entry.Metadata.ClrType.GetCustomAttribute<AuditedAttribute>();
        if (audited is null)
        {
            return null;
        }

        var added = entry.State == EntityState.Added;
        var before = added ? null : Snapshot(entry, audited, original: true);
        var after = Snapshot(entry, audited, original: false);
        var verb = added ? "Creó" : "Modificó";
        return new AuditRecord(audited.Module, added ? "Create" : "Update",
            $"{verb} {audited.Subject}.", entry.Metadata.ClrType.Name, entry.Entity.Id, before, after);
    }

    private static Dictionary<string, object?> Snapshot(
        EntityEntry<Entity> entry, AuditedAttribute audited, bool original) =>
        audited.Properties.ToDictionary(name => name, name =>
        {
            var property = entry.Property(name);
            return original ? property.OriginalValue : property.CurrentValue;
        });
}
