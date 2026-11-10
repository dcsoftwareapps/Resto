using Resto.Domain.Platform;

namespace Resto.Domain.Users;

[Audited("Users", "el rol", nameof(Name), nameof(IsSystem), nameof(IsActive))]
public sealed class Role : TenantEntity
{
    public Role(Guid organizationId, Guid restaurantId)
    {
        // The tenant identifiers are part of the key used by users and grants.
        ArgumentOutOfRangeException.ThrowIfEqual(organizationId, Guid.Empty);
        ArgumentOutOfRangeException.ThrowIfEqual(restaurantId, Guid.Empty);
        OrganizationId = organizationId;
        RestaurantId = restaurantId;
    }

    public required string Name { get; set; }
    public bool IsSystem { get; set; }
}
