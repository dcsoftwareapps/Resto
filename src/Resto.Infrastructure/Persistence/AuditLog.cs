using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Resto.Domain.Platform;
using Resto.Domain.Platform.Abstractions;

namespace Resto.Infrastructure.Persistence;

public sealed class AuditLog(AppDbContext context) : IAuditLog
{
    public async Task WriteAsync(AuditRecord record, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var entry = CreateEntry(context, record);
        context.AuditEntries.Add(entry);
        try
        {
            await context.SaveChangesAsync(ct);
        }
        catch
        {
            context.Entry(entry).State = EntityState.Detached;
            throw;
        }
    }

    internal static AuditEntry CreateEntry(AppDbContext context, AuditRecord record)
    {
        var actor = context.RequireCurrentUser();
        ArgumentException.ThrowIfNullOrWhiteSpace(record.Module);
        ArgumentException.ThrowIfNullOrWhiteSpace(record.Action);
        ArgumentException.ThrowIfNullOrWhiteSpace(record.Detail);

        return new AuditEntry
        {
            OrganizationId = actor.OrganizationId,
            RestaurantId = actor.RestaurantId,
            UserId = actor.UserId,
            DeviceId = actor.DeviceId,
            OccurredAt = DateTimeOffset.UtcNow,
            Module = record.Module,
            Action = record.Action,
            Detail = record.Detail,
            EntityType = record.EntityType,
            EntityId = record.EntityId,
            BeforeJson = record.Before is null ? null : JsonSerializer.Serialize(record.Before),
            AfterJson = record.After is null ? null : JsonSerializer.Serialize(record.After)
        };
    }
}
