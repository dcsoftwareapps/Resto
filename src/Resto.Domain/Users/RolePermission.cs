using Resto.Domain.Platform;

namespace Resto.Domain.Users;

[Audited("Users", "el permiso del rol", nameof(RoleId), nameof(Permission), nameof(IsActive))]
public sealed class RolePermission : TenantEntity
{
    public Guid RoleId { get; set; }
    public required string Permission { get; set; }
}
