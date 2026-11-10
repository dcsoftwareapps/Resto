using Resto.Domain.Platform;

namespace Resto.Domain.Users;

[Audited("Users", "el usuario", nameof(FullName), nameof(Initials), nameof(RoleId), nameof(IsActive))]
public sealed class User : TenantEntity
{
    public required string FullName { get; set; }
    public required string Initials { get; set; }
    public Guid RoleId { get; set; }
    public required string PinHash { get; set; }
    public string? Email { get; set; }
    public string? PasswordHash { get; set; }
}
