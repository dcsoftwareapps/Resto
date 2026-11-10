namespace Resto.Domain.Platform.Abstractions;

public interface IAuditLog
{
    Task WriteAsync(AuditRecord record, CancellationToken ct);
}

public sealed record AuditRecord(string Module, string Action, string Detail,
    string? EntityType = null, Guid? EntityId = null, object? Before = null, object? After = null);
