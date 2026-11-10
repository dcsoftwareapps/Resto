namespace Resto.Domain.Platform;

public sealed class AuditEntry : TenantEntity
{
    public Guid UserId { get; init; }
    public Guid? DeviceId { get; init; }
    public DateTimeOffset OccurredAt { get; init; }
    public required string Module { get; init; }
    public required string Action { get; init; }
    public required string Detail { get; init; }
    public string? EntityType { get; init; }
    public Guid? EntityId { get; init; }
    public string? BeforeJson { get; init; }
    public string? AfterJson { get; init; }
}
