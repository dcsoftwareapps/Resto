using Resto.Domain.Platform;

namespace Resto.Domain.Users;

public sealed class RolePermission : TenantEntity
{
    public Guid RoleId { get; set; }
    public required string Permission { get; set; }
}
